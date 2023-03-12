using Microsoft.AspNetCore.Mvc.ModelBinding;
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

namespace WarOfMinds.WebApi.SignalR
{

    public class TriviaHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;
        private readonly ISubjectService _subjectService;
        private static Dictionary<int, List<AnswerResult>> gameResults = new Dictionary<int, List<AnswerResult>>();
        public List<Question> questions { get; set; }
        public GameDTO game { get; set; }
        public TriviaHub(IGameService gameService, IPlayerService playerService, ISubjectService subjectService)
        {
            _gameService = gameService;
            _playerService = playerService;
            _subjectService = subjectService;
        }

        public async Task JoinGameAsync(int playerId, int subjectId)
        {
            PlayerDTO player = await _playerService.GetByIdAsync(playerId);
            SubjectDTO subject = await _subjectService.GetByIdAsync(subjectId);
            //בהערה עד שהאפדייט יעבוד ואז לשנות גם את הגיים אי די
            game = await _gameService.FindGameAsync(subject, player);
            //questions = new List<Question>();//הקריאה לאי פי אי של השאלות והמיפוי של הג'יסון לשאלה
            // Add player to game's SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{game.GameID}");

            // Send a message to the group            
            await Clients.Group($"game_{game.GameID}").SendAsync("ReceiveMessage", "my app", $"player {player.PlayerName} has joined the game{1} in subject {subject.Subjectname}.");
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

        public async void Execute()
        {                       
            foreach (Question item in questions)
            {
                //שולח את השאלה לכל השחקנים
                await DisplayQuestionAsync(item);
                //כאן השהיה של כמה שניות לקבלת התשובות
                //שולח את התשובה לכל השחקנים
                //חישוב הניקוד של השאלה הזו עבור כל השחקנים
                string winner=SortPlayersByAnswers(item.qNum);
                sendAnswerAndWinner(winner,item);
            }
            Scoring();
        }

        private string SortPlayersByAnswers(int qNum)
        {
            //מיון התשובות לפי נכונות וזמן
            //שליפת השם של השחקן המנצח
            List<AnswerResult> answers = gameResults[qNum];
            answers.Sort();
            return answers[answers.Count-1].ToString();//שליפת השחקן
        }

        private async void sendAnswerAndWinner(string winner,Question q)
        {
            //כדאי לשלוח את השם של השחקן שענה נכון ראשון
            //צריך למיין את השחקנים לפי המהירות והנכונות ולשלוף את השם של השחקן הראשון
            //כרגע שולח רק את התשובה
            await Clients.Group($"game_{game.GameID}").SendAsync("ReceiveAnswerAndWinner", q.correct_answer);
        }



        private void GetAnswerAsync(int qNum, string answer, int time)
        {
            Question q = questions[qNum];
            AnswerResult result = new AnswerResult(); // example AnswerResult object
            result.Score = result.IsCorrect(q.correct_answer, answer);
            result.player = Context.ConnectionId;//כרגע זה ה קונקשן אידי- זה אמור מתי שהוא להיות ממופה לפלייר מהדאטה בייס
            result.AnswerTime = time;//ההפרש בין הזמן שהוא קיבל את השאלה לבין הזמן שהוא שלח את התשובה.
            if (!gameResults.ContainsKey(qNum))
            {
                // If the key does not exist in the dictionary, create a new list and add it to the dictionary
                gameResults[qNum] = new List<AnswerResult>();
            }

            // Add the AnswerResult object to the list at the specified key
            gameResults[qNum].Add(result);

        }

        

        private void Scoring()
        {
            //בפונקציה הזו אמורים לחשב את המנצח, לשלוח לו את ההודעה המשמחת ולשלוח לכל השחקנים את הודעת הניצחון
            //לעדכן בדאטה בייס את הניקוד של כל השחקנים
            throw new NotImplementedException();
        }


        [HubMethodName("ReceiveQuestion")]
        private async Task DisplayQuestionAsync(Question question)
        {
            //שולח את השאלה כולל התשובה! צריך לטפל בזה
            //שליחת השאלה לכל השחקנים
            // Send a message to the group
            await Clients.Group($"game_{game.GameID}").SendAsync("ReceiveQuestion", question);

        }




    }
}
