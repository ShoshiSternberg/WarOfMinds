using WarOfMinds.Common.DTO;
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
            IEloCalculator eloCalculator, IDictionary<string, UserConnection> connections, IDictionary<string, GroupData> groupData, IHubContext<TriviaHub> hubContext, IConfiguration configuration)
        {
            _gameService = gameService;
            _playerService = playerService;
            _subjectService = subjectService;
            _eloCalculator = eloCalculator;
            _connections = connections;
            _groupData = groupData;
            _hubContext = hubContext;
           

            GameJoined += async (sender, e) =>
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "the IHubContext is calling for you");
                GameDTO g = await _gameService.GetByIdAsync(e.GameID);
                if (_groupData[$"game_{e.GameID}"].questions == null)//זה אומר שזו הפעם הראשונה שבוחרים את הנושא- תחילת המשחק
                {
                    await GetQuestionsAsync(e.GameID, e.SubjectID, _gameService.Difficulty(e.Rating));
                    
                    g = await _gameService.GetByIdAsync(e.GameID);
                    await Execute(e);                    
                }
            };
            _hubContext = hubContext;
            _configuration = configuration.GetSection("TriviaHub");
            _TimeToAnswer = _configuration.GetValue<int>("TimeToAnswer");
        }

        //יצירה וטיפול באירוע של הצטרפות למשחק
        public delegate void GameEventHandler(object sender, GameDTO e);

        public event GameEventHandler GameJoined;


        protected virtual void OnGameJoined(GameDTO e)
        {
            GameJoined?.Invoke(this, e);
        }

        public async Task JoinGameAsync(int playerId, int subjectId)
        {
            PlayerDTO player = await _playerService.GetByIdAsync(playerId);
            SubjectDTO subject = await _subjectService.GetByIdAsync(subjectId);
            GameDTO game = await _gameService.FindGameAsync(subject, player);
            if (game == null)
            {
                return;
            }
           
            if (game.Players.Contains(player))
            {
                //אם הוא מנסה להצטרף לאותו משחק שוב
                await Clients.Caller.SendAsync("ReceiveMessage", "You may not join the same game twice!");
            }
            else
            {
                //אם הוא הראשון שמצטרף למשחק
                lock (_groupData)
                {
                    if (!_groupData.ContainsKey($"game_{game.GameID}"))
                    {
                        _groupData.Add($"game_{game.GameID}", new GroupData());

                    }
                }

                lock (_connections)
                {
                    _connections.Add(Context.ConnectionId, new UserConnection(player, game.GameID));
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{game.GameID}");
                await Clients.Group($"game_{game.GameID}").SendAsync("ReceiveMessage", "my app", $"player {player.PlayerName} has joined the game{game.GameID} in subject {subject.Subjectname}.");

                if (_groupData[$"game_{game.GameID}"].IsActive == false)
                {
                    _groupData[$"game_{game.GameID}"].IsActive = true;
                    
                    OnGameJoined(game);
                }
                Console.WriteLine($"player {player.PlayerID} joined to game {game.GameID}");
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Clients.Group($"game_{_connections[Context.ConnectionId].game}").SendAsync("ReceiveMessage", "my app", $"player {_connections[Context.ConnectionId].player.PlayerName} has left the game{_connections[Context.ConnectionId].game}.");

            return base.OnDisconnectedAsync(exception);
        }


        //קבלת השאלות מהרשת, המרה לליסט של אובייקטים מסוג שאלה
        [HttpGet("{subject},{diffuculty}", Name = "GetRanking")]
        public async Task GetQuestionsAsync(int gameId, int subject, string difficulty)
        {
            try
            {                
                subject = 21;//למחוק את זה , צריך לשלוח רק אי די גדול מ11
                int amount = 10;//מספר השאלות            
                var client = new RestClient($"https://opentdb.com/api.php?amount={amount}&category={subject}&difficulty={difficulty}");                
                var request = new RestRequest("", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);
                //context 
                GameDTO g = await _gameService.GetByIdInNewScopeAsync(gameId);
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
        public async Task Execute(GameDTO game)
        {
            try
            {
                GameDTO g = await _gameService.GetByIdAsync(game.GameID);
                int timeToAnswer = _TimeToAnswer * 1000; //10 שניות
                Random rnd = new Random();
                rnd.Next();
                foreach (Question item in _groupData[$"game_{game.GameID}"].questions)
                {
                    g = await _gameService.GetByIdAsync(game.GameID);
                    //שולח את השאלה לכל השחקנים
                    await DisplayQuestionAsync(game.GameID, item);
                    //כאן השהיה של כמה שניות לקבלת התשובות
                    
                    await Task.Delay(10000);
                    
                    //שולח את התשובה לכל השחקנים
                    //חישוב הניקוד של השאלה הזו עבור כל השחקנים
                    string winner = SortPlayersByAnswers(game.GameID, item.questionId);
                    await ReceiveAnswerAndWinner(game.GameID, winner, item);
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
                await Clients.Caller.SendAsync("ReceiveAnswerAndWinner", $" Your answer has been captured in the system [{q.correct_answer == answer}], the correct answer is:{_groupData[$"game_{_connections[Context.ConnectionId].game}"].questions[qNum].correct_answer}");
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
                List<int> scores = players.Select(p => GetUserConnectionByPlayerID(p).score).ToList();
                int maxScore = scores.Max();
                if (maxScore > 0)
                {
                    string winner = players.FirstOrDefault(p => GetUserConnectionByPlayerID(p).score == maxScore).PlayerName;
                    await DisplayWinnerAndEndGameAsync(gameId, winner);
                }
                _eloCalculator.UpdateRatingOfAllPlayers(gameId, players, scores);

                //איכשהו לשלוח לשחקן את הציון המעודכן שלו.
                //אולי בסיום דרך הקונטרולר
            }catch(Exception e)
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

        //פונקציה שהקליינט  מוכן אליה לקבלת שאלה
        [HubMethodName("ReceiveQuestion")]
        private async Task DisplayQuestionAsync(int gameId, Question question)
        {
            Random rand = new Random();
            int randomInt = rand.Next(4);//מגרילים מקום לתשובה הנכונה
            //שולח את השאלה בלי התשובה
            //question.correct_answer = null;
            Question q = new Question();
            q.question = question.question;
            q.questionId = question.questionId;
            q.incorrect_answers = question.incorrect_answers;
            q.incorrect_answers.Insert(randomInt, question.correct_answer);//מוסיפים את  התשובה הנכונה לרשימת התשובות
            q.difficulty = question.difficulty;
            q.category = question.category;

            //שליחת השאלה לכל השחקנים
            // Send a message to the group
            await _hubContext.Clients.Group($"game_{gameId}").SendAsync("ReceiveQuestion", q);

        }
        // פונקציה  שהקליינט מתכונן אליה לקבלת תוצאות המשחק
        [HubMethodName("ReceiveWinnerAndGameEnd")]
        private async Task DisplayWinnerAndEndGameAsync(int gameId, string p1)
        {
            //שליחת המנצחים
            await _hubContext.Clients.Group($"game_{gameId}").SendAsync("ReceiveQuestion", $"first place: {p1} ");
            //שליחת הנקודות שהוא צבר במשחק הזה
            //וכן הדירוג המעודכן שלו- אחרי החישוב
        }




    }

}
