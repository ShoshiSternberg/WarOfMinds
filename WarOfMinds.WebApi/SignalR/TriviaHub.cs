using Microsoft.AspNetCore.SignalR;
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

        public TriviaHub(IGameService gameService, IPlayerService playerService, ISubjectService subjectService, IEloCalculator eloCalculator, IDictionary<string, UserConnection> connections, IDictionary<string, GroupData> groupData)
        {
            _gameService = gameService;
            _playerService = playerService;
            _subjectService = subjectService;
            _eloCalculator = eloCalculator;
            _connections = connections;
            _groupData = groupData;
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
            //מילוי השאלות רק בפעם הראשונה
            if (_groupData[$"game_{game.GameID}"].questions == null)
            {
                await GetQuestionsAsync(subject.SubjectID, _gameService.Difficulty(game.Rating));
                //מיד אחרי שמכניסים את השאלות, מתחילים את המשחק 
                Execute();
            }

        }



        [HttpGet("{subject},{diffuculty}", Name = "GetRanking")]
        public async Task GetQuestionsAsync(int subject, string difficulty)
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
            _groupData[$"game_{_connections[Context.ConnectionId].game}"].questions = questionsList.results;
            // Set questionId for each Question object
            int questionId = 1;
            foreach (var question in questionsList.results)
            {
                question.questionId = questionId++;
            }

        }


        //להמשיך ולשאול את GPT בקשר ללולאה עם הטיימר
        //public async Task GameProgressAsync(int gameId)
        //{
        //    // Get the game by ID
        //    GameDTO game = await _gameService.GetByIdAsync(gameId);

        //    if (game == null)
        //    {
        //        // Game not found, handle error or return
        //        return;
        //    }

        //    // Set up the game loop
        //    var questions = game.Subject.Questions;
        //    var scores = new Dictionary<PlayerDTO, List<AnswerResult>>();

        //    for (int i = 0; i < questions.Count; i++)
        //    {

        //        // Send question to players in the group
        //        await Clients.Group($"game_{gameId}").SendAsync("ReceiveQuestion", questions[i]);
        //        DateTime questionStartTime = DateTime.UtcNow;
        //        // Wait for players to answer
        //        var timeToAnswer = TimeSpan.FromSeconds(30);
        //        var answers = new List<PlayerAnswer>();
        //        var timer = new Timer(async _ =>
        //        {
        //            // Get answers and calculate scores
        //            answers = await _dbContext.PlayerAnswers.Include(pa => pa.Player)
        //                                                     .Where(pa => pa.GameID == game.GameID && pa.QuestionID == questions[i].QuestionID)
        //                                                     .ToListAsync();
        //            //עובר על כל התשובות
        //            foreach (var answer in answers)
        //            {
        //                //אם עדין אין את השחקן הזה- מוסיף אותו
        //                if (!scores.ContainsKey(answer.Player))
        //                {
        //                    scores[answer.Player] = new List<AnswerResult>();
        //                }
        //                //מוסיף תשובה למילון שבו המפתח הוא השחקן והערך הוא ליסט של תשובות

        //                AnswerResult answerResult = new AnswerResult();
        //                answerResult.Score = answerResult.IsCorrect(answer.answerId, answer.playerAnswer);
        //                answerResult.AnswerTime = DateTime.UtcNow - questionStartTime;//הזמן הנוכחי פחות זמן שליחת השאלה

        //                scores[answer.Player].Add(answerResult);
        //            }
        //        }, null, timeToAnswer, System.Threading.Timeout.InfiniteTimeSpan);

        //        // Wait for players to answer
        //        await Task.Delay(timeToAnswer);

        //        // Cancel the timer and get the final list of answers
        //        timer.Dispose();

        //        // Send answers to players in the group
        //        await Clients.Group($"game_{gameId}").SendAsync("ReceiveAnswers", answers);
        //    }

        //    //// End of game, calculate winner
        //    //var winner = scores.OrderByDescending(s => s.Value.Sum(a => a.Score ? a.AnswerTime : 0)).FirstOrDefault();

        //    //if (winner != null)
        //    //{
        //    //    // Send results to players in the group
        //    //    await Clients.Group($"game_{gameId}").SendAsync("GameResults", scores, winner.Key);
        //    //}

        //    //// Remove players from the group for the game
        //    //await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        //}

        //public async Task SubmitAnswer(string gameId, int questionId, string answer)
        //{
        //    StringValues groupId = Context.GetHttpContext().Request.Query["groupId"];



        //    var player = game.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

        //    if (player == null)
        //    {
        //        // Player not found, handle error or return
        //        return;
        //    }
        //    if (!scores.ContainsKey(answer.Player))
        //    {
        //        scores[answer.Player] = new List<AnswerResult>();
        //    }
        //    //מוסיף תשובה למילון שבו המפתח הוא השחקן והערך הוא ליסט של תשובות

        //    AnswerResult answerResult = new AnswerResult();
        //    answerResult.Score = answerResult.IsCorrect(answer.answerId, answer.playerAnswer);
        //    answerResult.AnswerTime = DateTime.UtcNow - questionStartTime;//הזמן הנוכחי פחות זמן שליחת השאלה

        //    scores[answer.Player].Add(answerResult);
        //    // Save the answer to the database
        //    var playerAnswer = new PlayerAnswer
        //    {
        //        GameID = game.GameID,
        //        PlayerID = player.PlayerID,
        //        QuestionID = questionId,
        //        Answer = answer,
        //        TimeToAnswer = (int)(DateTime.UtcNow - questionStartTime).TotalSeconds
        //    };

        //    _dbContext.PlayerAnswers.Add(playerAnswer);
        //    await _dbContext.SaveChangesAsync();
        //}

        public async Task Execute()
        {
            int timeToAnswer = 10000; //10 שניות
            Random rnd = new Random();
            rnd.Next();
            foreach (Question item in _groupData[$"game_{_connections[Context.ConnectionId].game}"].questions)
            {
                //שולח את השאלה לכל השחקנים
                await DisplayQuestionAsync(item);
                //כאן השהיה של כמה שניות לקבלת התשובות
                //if (Context == null) { return; }
                //await Task.Delay(timeToAnswer);
                //if (Context == null) { return; }

                await GetAnswerAsync(item.questionId, $"answer{item.questionId}", rnd.Next());

                //שולח את התשובה לכל השחקנים
                //חישוב הניקוד של השאלה הזו עבור כל השחקנים
                string winner = SortPlayersByAnswers(item.questionId);
                ReceiveAnswerAndWinner(winner, item);
            }
            Scoring();
        }

        public string SortPlayersByAnswers(int qNum)
        {
            if (Context.ConnectionAborted.IsCancellationRequested)
            {
                return "connection closed";
            }


            if (_groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults != null)
            {
                if (_groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults.ContainsKey(qNum))
                {
                    List<AnswerResult> answers = _groupData[$"game_{_connections[Context.ConnectionId].game}"].gameResults[qNum];
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

        public async Task ReceiveAnswerAndWinner(string winner, Question q)
        {
            //כדאי לשלוח את השם של השחקן שענה נכון ראשון

            //כרגע שולח רק את התשובה
            await Clients.Group($"game_{_connections[Context.ConnectionId].game}").SendAsync("ReceiveAnswerAndWinner", q.correct_answer, winner);
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
            await Clients.Caller.SendAsync("ReceiveAnswer", $" Your answer has been captured in the system [{result.answer}], the correct answer is:{_groupData[$"game_{_connections[Context.ConnectionId].game}"].questions[qNum].correct_answer}");
        }



        private async Task Scoring()
        {
            //בפונקציה הזו אמורים לחשב את המנצח, (לשלוח לו את ההודעה המשמחת) ולשלוח לכל השחקנים את הודעת הניצחון
            //לעדכן בדאטה בייס את הניקוד של כל השחקנים
            //המנצח הוא מי שקיבל הכי הרבה נקודות
            List<PlayerDTO> players = _groupData[$"game_{_connections[Context.ConnectionId].game}"].game.Players.ToList<PlayerDTO>();
            players = players.OrderBy(p => GetScoreByPlayerID(p.PlayerID)).ToList();
            await DisplayWinnerAndEndGameAsync(players[0].PlayerName, players[1].PlayerName, players[2].PlayerName);

            _eloCalculator.UpdateRatingOfAllPlayers(_connections[Context.ConnectionId].game, players);
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


        [HubMethodName("ReceiveQuestion")]
        private async Task DisplayQuestionAsync(Question question)
        {
            //שולח את השאלה בלי התשובה
            question.correct_answer = null;
            //שליחת השאלה לכל השחקנים
            // Send a message to the group
            await Clients.Group($"game_{_connections[Context.ConnectionId].game}").SendAsync("ReceiveQuestion", question);

        }

        [HubMethodName("ReceiveWinnerAndGameEnd")]
        private async Task DisplayWinnerAndEndGameAsync(string p1, string p2, string p3)
        {
            //שליחת המנצחים
            await Clients.Group($"game_{_connections[Context.ConnectionId].game}").SendAsync("ReceiveQuestion", $"first place: {p1} second place: {p2} third place: {p3}");
            //שליחת הנקודות שהוא צבר במשחק הזה
            //וכן הדירוג המעודכן שלו- אחרי החישוב
        }




    }
}
