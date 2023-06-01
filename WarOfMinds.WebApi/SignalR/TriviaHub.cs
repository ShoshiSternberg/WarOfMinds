﻿using WarOfMinds.Common.DTO;
using WarOfMinds.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Data;
using Microsoft.AspNetCore.SignalR;
using RestSharp;
using RestSharp.Authenticators;
using System.Linq;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace WarOfMinds.WebApi.SignalR
{

    public class TriviaHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;
        private readonly ISubjectService _subjectService;
        private readonly IEloCalculator _eloCalculator;
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IDictionary<string, GroupData> _groupData;
        private readonly IHubContext<TriviaHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly int _TimeToAnswer;


        public TriviaHub(IGameService gameService, IPlayerService playerService, ISubjectService subjectService,
            IEloCalculator eloCalculator, IDictionary<string, UserConnection> connections, IDictionary<string, GroupData> groupData,
            IHubContext<TriviaHub> hubContext, IConfiguration configuration)
        {
            _gameService = gameService;
            _playerService = playerService;
            _subjectService = subjectService;
            _eloCalculator = eloCalculator;
            _connections = connections;
            _groupData = groupData;
            _hubContext = hubContext;
            _hubContext = hubContext;
            _configuration = configuration.GetSection("TriviaHub");

            GameStarted += async (sender, e) =>
            {

                if (_groupData[$"game_{e.GameID}"].questions == null)
                {
                    await GetQuestionsAsync(e.GameID, e.SubjectID, "hard");
                    await ExecuteAsync(e);
                }
            };

        }

        //יצירה וטיפול באירוע של הפעלת למשחק
        public delegate void GameEventHandler(object sender, GameDTO e);

        public event GameEventHandler GameStarted;


        protected virtual void OnGameStarted(GameDTO e)
        {
            GameStarted?.Invoke(this, e);
        }


        // הצטרפות למשחק קיים
        public async Task JoinExistingGameAsync(int playerId, int subjectId)
        {
            PlayerDTO player = await _playerService.GetByIdAsync(playerId);
            SubjectDTO subject = await _subjectService.GetByIdAsync(subjectId);
            GameDTO game = await _gameService.FindActiveGameAsync(subject, player);
            if (game == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", $"there are not active game in subject {subject.Subjectname} with your rating.");

            }
            if (_connections.Values.Any(p => (p.player == player) && (p.game == game.GameID)))
            {
                //אם הוא מנסה להצטרף לאותו משחק שוב
                await Clients.Caller.SendAsync("ReceiveMessage", "You may not join the same game twice!");
            }
            else
            {
                _connections.Add(Context.ConnectionId, new UserConnection(player, game.GameID));

                await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{game.GameID}");
                await Clients.Group($"game_{game.GameID}").SendAsync("PlayerJoined", player);
                List<PlayerDTO> members = _connections.Values.ToList().Where(e => e.game == game.GameID && !(e.player.PlayerID == playerId)).Select(e => e.player).ToList();
                await Clients.Caller.SendAsync("JoinWaitingRoom", members);
                //$"player {player.PlayerName} has joined the game{game.GameID} in subject {subject.Subjectname}.");

                Console.WriteLine($"player {player.PlayerID} joined to game (in process) {game.GameID}");
            }
        }

        // פתיחת חדר חדש
        public async Task CreateNewGameAsync(int playerId, int subjectId)
        {
            PlayerDTO player = await _playerService.GetByIdAsync(playerId);
            SubjectDTO subject = await _subjectService.GetByIdAsync(subjectId);
            GameDTO game = new GameDTO();
            game.Subject = subject;
            game.SubjectID = subjectId;
            game.GameDate = DateTime.Now;
            game.Rating = player.ELORating;
            game.IsActive = true;
            game.OnHold = true;
            game.Players = new List<PlayerDTO>();
            game.Players.Add(player);
            game = await _gameService.AddGameAsync(game);
            if (game == null)
            {
                return;
            }
            if (_connections.Values.Any(p => (p.player == player) && (p.game == game.GameID)))
            {
                //אם הוא מנסה להצטרף לאותו משחק שוב
                await Clients.Caller.SendAsync("ReceiveMessage", "You may not join the same game twice!");
            }
            else
            {
                //לא בטוח שצריך את הנעילה
                lock (_groupData)
                {
                    if (!_groupData.ContainsKey($"game_{game.GameID}"))
                    {
                        _groupData.Add($"game_{game.GameID}", new GroupData());
                    }
                }
                //אם הוא הראשון שהצטרף- המשך הטיפול מחוץ לקטע הקריטי
                if (_groupData[$"game_{game.GameID}"].game == null)
                {
                    _groupData[$"game_{game.GameID}"].GameManagerConnectionID = Context.ConnectionId;
                    _groupData[$"game_{game.GameID}"].game = game;
                    await _gameService.UpdateGameAsync(game.GameID, _groupData[$"game_{game.GameID}"].game);
                }
                //לא צריך לנעול כי בכל מקרה זה מפתחות שונים              

                _connections.Add(Context.ConnectionId, new UserConnection(player, game.GameID));

                await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{game.GameID}");
            }
                                
            List<PlayerDTO> members = _connections.Values.ToList().Where(e => e.game == game.GameID).Select(e => e.player).ToList();
            await Clients.Caller.SendAsync("JoinWaitingRoom", members);
            await Clients.Group($"game_{game.GameID}").SendAsync("PlayerJoined", player);
            Console.WriteLine($"player {player.PlayerID} open new waiting room to game {game.GameID}");
        }

        //הצטרפות לחדר המתנה
        public async Task JoinWaitingRoomAsync(int playerId, int subjectId)
        {
            PlayerDTO player = await _playerService.GetByIdAsync(playerId);
            SubjectDTO subject = await _subjectService.GetByIdAsync(subjectId);
            GameDTO game = await _gameService.FindOnHoldGameAsync(subject, player);
            if (game == null)
            {
                await CreateNewGameAsync(playerId, subjectId);//אם לא מצאו חדר בהמתנה פותחים חדר חדש
                return;
            }
            else
            {
                if (_connections.Values.Any(p => (p.player == player) && (p.game == game.GameID)))
                {
                    //אם הוא מנסה להצטרף לאותו משחק שוב
                    await Clients.Caller.SendAsync("ReceiveMessage", "You may not join the same game twice!");
                }
                else
                {
                    //לא צריך לנעול כי בכל מקרה זה מפתחות שונים          
                    _connections.Add(Context.ConnectionId, new UserConnection(player, game.GameID));

                    await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{game.GameID}");

                    List<PlayerDTO> members = _connections.Values.ToList().Where(e => e.game == game.GameID && !(e.player.PlayerID == playerId)).Select(e => e.player).ToList();
                    await Clients.Caller.SendAsync("JoinWaitingRoom", members);
                    await Clients.Group($"game_{game.GameID}").SendAsync("PlayerJoined", player);
                    Console.WriteLine($"player {player.PlayerID} joined to waiting room for game {game.GameID}");
                }
            }
        }

        //הפעלת המשחק על ידי מי שפתח את החדר
        public async Task StartGameByManager()
        {
            PlayerDTO player = _connections[Context.ConnectionId].player;
            GameDTO game = _groupData[$"game_{_connections[Context.ConnectionId].game}"].game;

            if (_groupData[$"game_{game.GameID}"].GameManagerConnectionID == Context.ConnectionId)
            {
                _groupData[$"game_{game.GameID}"].IsActive = true;
                game.OnHold = false;
                await _gameService.UpdateGameAsync(game.GameID, game);
                OnGameStarted(game);
            }

        }


        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    Clients.Group($"game_{_connections[Context.ConnectionId].game}").SendAsync("ReceiveMessage", "my app", $"player {_connections[Context.ConnectionId].player.PlayerName} has left the game{_connections[Context.ConnectionId].game}.");

        //    return base.OnDisconnectedAsync(exception);
        //}


        //קבלת השאלות מהרשת, המרה לליסט של אובייקטים מסוג שאלה
        [HttpGet("{subject},{diffuculty}", Name = "GetRanking")]
        public async Task GetQuestionsAsync(int gameId, int subject, string difficulty)
        {
            try
            {
                int amount = _configuration.GetValue<int>("NumOfQuestions");//מספר השאלות            
                var client = new RestClient($"https://opentdb.com/api.php?amount={amount}&category={subject}&difficulty={difficulty}");
                var request = new RestRequest("", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);

                string jsonString = response.Content;
                //המרה מג'יסון לאובייקט שאלה
                Root questionsList =
                    JsonSerializer.Deserialize<Root>(jsonString);

                if (questionsList.results == null)//אם אין אינטרנט בינתיים שיקרא מהדף
                {
                    string text = File.ReadAllText(@"H:\תכנות שנה ב תשפג\שושי שטרנברגר\פרויקט גמר\finnal project\WarOfMinds\WarOfMinds.WebApi\SignalR\Questions.json");
                    questionsList = JsonSerializer.Deserialize<Root>(text);
                }
                _groupData[$"game_{gameId}"].questions = questionsList.results;
                // Set questionId for each Question object
                int questionId = 0;
                foreach (var question in questionsList.results)
                {
                    question.questionId = questionId++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }



        //מהלך המשחק
        public async Task ExecuteAsync(GameDTO game)
        {
            try
            {
                int timeToAnswer = _configuration.GetValue<int>("timeToAnswer") * 1000; //10 שניות
                int timeToSeeAnswer = _configuration.GetValue<int>("timeToSeeAnswer") * 1000;
                Random rnd = new Random();
                rnd.Next();
                foreach (Question item in _groupData[$"game_{game.GameID}"].questions)
                {

                    //שולח את השאלה לכל השחקנים
                    await DisplayQuestionAsync(game.GameID, item);
                    //כאן השהיה של כמה שניות לקבלת התשובות

                    await Task.Delay(timeToAnswer);

                    //שולח את התשובה לכל השחקנים
                    //חישוב הניקוד של השאלה הזו עבור כל השחקנים
                    string winner = SortPlayersByAnswers(game.GameID, item.questionId);
                    await ReceiveAnswerAndWinner(game.GameID, winner, item);
                    await Task.Delay(timeToSeeAnswer);//השהיה של כמה שניות להצגת התשובה
                }
                await Scoring(game.GameID);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string SortPlayersByAnswers(int gameId, int qNum)
        {
            if (_groupData[$"game_{gameId}"].gameResults != null)
            {
                //אם יש שחקנים שענו נכון על השאלה ממינים אותם לפי סדר המענה ושולפים את השחקן הראשון שזמן המענה שלו היה הכי נמוך

                //ברשימה הזו יש רק את מי שענה נכון על השאלה. צריך למיין את הרשימה לפי זמן מענה, לראות מי המנצח ולעדכן לכל השחקנים את כל הניקודים.
                List<AnswerResult> answers = _groupData[$"game_{gameId}"].gameResults;
                answers.Sort();//מיון התשובות לפי נכונות וזמן             

                return answers[0].player.PlayerName;//שליפת השם של השחקן המנצח

            }
            return "No one answered this question correctly :(";
        }

        public async Task ReceiveAnswerAndWinner(int gameId, string winner, Question q)
        {
            //כדאי לשלוח את השם של השחקן שענה נכון ראשון

            //כרגע שולח רק את התשובה
            await _hubContext.Clients.Group($"game_{gameId}").SendAsync("ReceiveAnswerAndWinner", q.correct_answer, winner);
        }

        public async Task GetAnswerAsync(int qNum, string answer, int time)
        {
            try
            {
                Question q = _groupData[$"game_{_connections[Context.ConnectionId].game}"].questions[qNum];
                if (q.correct_answer == answer)
                {
                    int score = _TimeToAnswer - time + 1;//על כל שניה של איחור מפסידים נקודה
                    _connections[Context.ConnectionId].score += score;//מוסיפים את הנקודות על השאלה הזו לניקוד בכל המשחק
                    AnswerResult result = new AnswerResult();
                    result.connectionId = Context.ConnectionId;
                    result.player = _connections[Context.ConnectionId].player;
                    result.qNum = qNum;
                    result.AnswerTime = time;//ההפרש בין הזמן שהוא קיבל את השאלה לבין הזמן שהוא שלח את התשובה.
                    if (_groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults == null)
                    {
                        lock (_groupData[$"game_{_connections[Context.ConnectionId].game}"])
                        {
                            //if it is the first question, we have to create a new dictionary
                            _groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults = new List<AnswerResult>();
                        }
                    }
                    lock (_groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults)
                    {
                        // Add the AnswerResult object to the list at the specified key
                        _groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults.Add(result);
                    }
                }
                //שולחים לו מייד אם ענה נכון או לא
                await Clients.Caller.SendAsync("ReceiveAnswerAndWinner",
                    $" Your answer has been captured in the system [{q.correct_answer == answer}], the correct answer is:{_groupData[$"game_{_connections[Context.ConnectionId].game}"].questions[qNum].correct_answer}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }




        private async Task Scoring(int gameId)
        {
            try
            {
                //שולפים את רשימת השחקנים מהדאטה בייס

                //יוצרים רשימה של כל הניקודים שהם צברו
                //בודקים מיהו השחקן שלו הניקוד הכי גבוה
                //שולחים אותו בתור מנצח
                //שולחים את כל השחקנים מהדאטה בייס+ את הניקודים שלהם+את הקוד של המשחק לחישוב ניקודים ועדכון דאטה בייס                
                List<PlayerDTO> players = (await _gameService.GetByIdInNewScopeAsync(gameId)).Players.ToList();
                if (players.Count < 2)
                {
                    if (GetUserConnectionByPlayerID(players[0]).score > _configuration.GetValue<Int32>("NumOfQuestions") * _configuration.GetValue<Int32>("TimeToAnswer"))
                    {

                    }

                }
                players.Sort((p1, p2) => (GetUserConnectionByPlayerID(p1).score).CompareTo(GetUserConnectionByPlayerID(p2).score));
                List<int> scores = players.Select(p => GetUserConnectionByPlayerID(p).score).ToList();
                int maxScore = scores.Max();
                if (GetUserConnectionByPlayerID(players[0]).score > 0)
                {
                    string winner = players[0].PlayerName;//.FirstOrDefault(p => GetUserConnectionByPlayerID(p).score == maxScore).PlayerName;
                    await DisplayWinnerAndEndGameAsync(gameId, players[0].PlayerName, players[1].PlayerName, players[2].PlayerName);
                }

                _eloCalculator.UpdateRatingOfAllPlayers(gameId, players, scores);

                //איכשהו לשלוח לשחקן את הציון המעודכן שלו.
                //אולי בסיום דרך הקונטרולר
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        //מציאת UserConnection לפי playerID
        private UserConnection GetUserConnectionByPlayerID(PlayerDTO player)
        {
            var userConnection = _connections.Values.FirstOrDefault(conn => conn.player.PlayerID == player.PlayerID);

            if (userConnection == null)
                return null;
            return userConnection;
        }


        [HubMethodName("ReceiveQuestion")]
        private async Task DisplayQuestionAsync(int gameId, Question question)
        {
            Random rand = new Random();
            int randomInt = rand.Next(4);//מגרילים מקום לתשובה הנכונה                     
            Question q = new Question();
            q.question = question.question;
            q.questionId = question.questionId;
            q.incorrect_answers = question.incorrect_answers;
            q.incorrect_answers.Insert(randomInt, question.correct_answer);//מוסיפים את  התשובה הנכונה לרשימת התשובות
            q.difficulty = question.difficulty;
            q.category = question.category;


            await _hubContext.Clients.Group($"game_{gameId}").SendAsync("ReceiveQuestion", q);

        }



        [HubMethodName("ReceiveWinnerAndGameEnd")]
        private async Task DisplayWinnerAndEndGameAsync(int gameId, string p1, string p2, string p3)
        {
            //שליחת המנצחים
            await _hubContext.Clients.Group($"game_{gameId}").SendAsync("ReceiveQuestion", $"first place: {p1} second place: {p2} third place: {p3}");

        }




    }

}
