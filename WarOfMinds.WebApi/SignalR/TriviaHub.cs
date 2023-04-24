using Microsoft.Identity.Client.Extensibility;
using System;
using System.Collections.Generic;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Services.Services;
using Microsoft.Extensions.Primitives;
using WarOfMinds.Services.Interfaces;
using System.Numerics;
using System.Xml.Linq;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using static Azure.Core.HttpHeader;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Immutable;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.SignalR;


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

        public TriviaHub(IGameService gameService, IPlayerService playerService, ISubjectService subjectService, IEloCalculator eloCalculator, IDictionary<string, UserConnection> connections, IDictionary<string, GroupData> groupData, IHubContext<TriviaHub> hubContext)
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
                if (_groupData[$"game_{e.GameID}"].questions == null)
                {
                    await GetQuestionsAsync(e.GameID,e.SubjectID, _gameService.Difficulty(e.Rating));
                    //מיד אחרי שמכניסים את השאלות, מתחילים את המשחק 

                    await Task.Run(async () =>
                    {
                        // Simulate some work
                        await Execute(e);
                    });


                }
            };
            _hubContext = hubContext;
        }
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
            //בהערה עד שהאפדייט יעבוד ואז לשנות גם את הגיים אי די
            GameDTO game = await _gameService.FindGameAsync(subject, player);
            //GameDTO game =await _gameService.GetByIdAsync(3);//בינתיים שולף ולא מעדכן
            // Add player to game's SignalR group

            await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{game.GameID}");
            
            _connections.Add(Context.ConnectionId, new UserConnection(player, game.GameID));
            if (!(_groupData.ContainsKey($"game_{game.GameID}")))
                _groupData.Add($"game_{game.GameID}", new GroupData());
            _groupData[$"game_{game.GameID}"].game = game;//עדכון המשחק בקבוצה שלו
            // Send a message to the group            
            await Clients.Group($"game_{game.GameID}").SendAsync("ReceiveMessage", "my app", $"player {player.PlayerName} has joined the game{game.GameID} in subject {subject.Subjectname}.");
            //onJoinGame
            OnGameJoined(game);
            //מילוי השאלות רק בפעם הראשונה
            //if (_groupData[$"game_{game.GameID}"].questions == null)
            //{
            //    await GetQuestionsAsync(subject.SubjectID, _gameService.Difficulty(game.Rating));
            //    //מיד אחרי שמכניסים את השאלות, מתחילים את המשחק 

            //    await Task.Run(async () =>
            //    {
            //        // Simulate some work
            //        await Execute();
            //    });

            //}
            Console.WriteLine("after the join game");

        }




        //קבלת השאלות מהרשת, המרה לליסט של אובייקטים מסוג שאלה
        [HttpGet("{subject},{diffuculty}", Name = "GetRanking")]
        public async Task GetQuestionsAsync(int gameId,int subject, string difficulty)
        {
            try
            {
                subject = 21;//למחוק את זה , צריך לשלוח רק אי די גדול מ11
                int amount = 10;//מספר השאלות            
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
                int questionId = 1;
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
                int timeToAnswer = 10000; //10 שניות
                Random rnd = new Random();
                rnd.Next();
                foreach (Question item in _groupData[$"game_{game.GameID}"].questions)
                {
                    //שולח את השאלה לכל השחקנים
                    await DisplayQuestionAsync(game.GameID, item);
                    //כאן השהיה של כמה שניות לקבלת התשובות

                    await Task.Delay(5000);

                    //שולח את התשובה לכל השחקנים
                    //חישוב הניקוד של השאלה הזו עבור כל השחקנים
                    string winner = SortPlayersByAnswers(game.GameID,item.questionId);
                    await ReceiveAnswerAndWinner(game.GameID,winner, item);
                }
                await Scoring(game.GameID);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string SortPlayersByAnswers(int gameId,int qNum)
        {
            

            if (_groupData[$"game_{gameId}"].gameResults != null)
            {
                if (_groupData[$"game_{gameId}"].gameResults.ContainsKey(qNum))
                {
                    List<AnswerResult> answers = _groupData[$"game_{gameId}"].gameResults[qNum];
                    answers.Sort();//מיון התשובות לפי נכונות וזמן
                    //עדכון הנקודות של כל שחקן על השאלה הזו- לפי המיקום שלו במערך. אם הוא ענה לא נכון
                    //הניקוד הוא 0
                    int j;
                    for (j = 0; answers[j].answer == true; j++) ;//סופר כמה ענו נכון
                    for (int i = 0; i < answers.Count; i++)
                    {
                        if (answers[i].answer == true)
                            _connections[answers[i].connectionId].score += j--;//ימ שענה הכי מהר מקבל הכי הרבה נקודות

                    }
                    if (answers[0].answer == true)//רק אם הוא באמת ענה נכון- יתכן שכל התשובות לא היו נכונות
                    {
                        return answers[0].player.PlayerName;//שליפת השם של השחקן המנצח
                    }
                }
            }
            return "No one answered this question correctly :(";
        }

        public async Task ReceiveAnswerAndWinner(int gameId,string winner, Question q)
        {
            //כדאי לשלוח את השם של השחקן שענה נכון ראשון

            //כרגע שולח רק את התשובה
            await _hubContext.Clients.Group($"game_{gameId}").SendAsync("ReceiveAnswerAndWinner", q.correct_answer, winner);
        }



        public async Task GetAnswerAsync(int qNum, string answer, int time)
        {
            Question q = _groupData[$"game_{_connections[Context.ConnectionId].game}"].questions[qNum];
            AnswerResult result = new AnswerResult();
            result.answer = result.IsCorrect(q.correct_answer, answer);
            result.connectionId = Context.ConnectionId;
            result.player = _connections[Context.ConnectionId].player;
            result.AnswerTime = time;//ההפרש בין הזמן שהוא קיבל את השאלה לבין הזמן שהוא שלח את התשובה.
            if (_groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults == null)
            {
                //if it is the first question, we have to create a new dictionary
                _groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults = new Dictionary<int, List<AnswerResult>>();
            }

            if (!_groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults.ContainsKey(qNum))
            {
                // If the key does not exist in the dictionary, create a new list and add it to the dictionary
                _groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults[qNum] = new List<AnswerResult>();
            }


            // Add the AnswerResult object to the list at the specified key
            _groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults[qNum].Add(result);
            //שולחים לו מייד אם ענה נכון או לא
            await Clients.Caller.SendAsync("ReceiveAnswerAndWinner", $" Your answer has been captured in the system [{result.answer}], the correct answer is:{_groupData[$"game_{_connections[Context.ConnectionId].game}"].questions[qNum].correct_answer}");
        }



        private async Task Scoring(int gameId)
        {
            //בפונקציה הזו אמורים לחשב את המנצח, (לשלוח לו את ההודעה המשמחת) ולשלוח לכל השחקנים את הודעת הניצחון
            //לעדכן בדאטה בייס את הניקוד של כל השחקנים
            //המנצח הוא מי שקיבל הכי הרבה נקודות
            List<PlayerDTO> players = _groupData[$"game_{gameId}"].game.Players.ToList<PlayerDTO>();
            players = players.OrderBy(p => GetScoreByPlayerID(p.PlayerID)).ToList();
            await DisplayWinnerAndEndGameAsync(gameId,players[0].PlayerName);

            _eloCalculator.UpdateRatingOfAllPlayers(gameId, players);
            //איכשהו לשלוח לשחקן את הציון המעודכן שלו.
            //אולי בסיום דרך הקונטרולר
        }

        //מציאת UserConnection לפי playerID
        private int GetScoreByPlayerID(int playerID)
        {
            var userConnection = _connections.Values.FirstOrDefault(conn => conn.player.PlayerID == playerID);

            if (userConnection == null)
                return 0;
            return userConnection.score;
        }

        //פונקציה שמפעיל הקליינט לקבלת שאלה
        [HubMethodName("ReceiveQuestion")]
        private async Task DisplayQuestionAsync(int gameId,Question question)
        {
            //שולח את השאלה בלי התשובה
            question.correct_answer = null;
            //שליחת השאלה לכל השחקנים
            // Send a message to the group
            await _hubContext.Clients.Group($"game_{gameId}").SendAsync("ReceiveQuestion", question);

        }
        // פונקציה שמפעיל הקליינט לקבלת תוצאות המשחק
        [HubMethodName("ReceiveWinnerAndGameEnd")]
        private async Task DisplayWinnerAndEndGameAsync(int gameId,string p1)
        {
            //שליחת המנצחים
            await _hubContext.Clients.Group($"game_{gameId}").SendAsync("ReceiveQuestion", $"first place: {p1} ");
            //שליחת הנקודות שהוא צבר במשחק הזה
            //וכן הדירוג המעודכן שלו- אחרי החישוב
        }




    }

}
