﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client.Extensibility;
using System;
using System.Collections.Generic;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Services.Services;
using Microsoft.Extensions.Primitives;
using WarOfMinds.Services.Interfaces;

namespace WarOfMinds.WebApi.SignalR
{

    public class TriviaHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;
        private readonly ISubjectService _subjectService;
        //private static Dictionary<string, List<AnswerResult>> gameResults = new Dictionary<string, List<AnswerResult>>();
        public TriviaHub(IGameService gameService, IPlayerService playerService, ISubjectService subjectService)
        {
            _gameService = gameService;
            _playerService = playerService;
            _subjectService = subjectService;
        }

        public async Task JoinGameAsync(int playerId, int subjectId)
        {
            var player = await _playerService.GetByIdAsync(playerId);
            var subject = await _subjectService.GetByIdAsync(subjectId);
            var game = await _gameService.FindGameAsync(subject, player);

            // Add player to game's SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{subjectId}");

            // Send a message to the group
            await Clients.Group($"game_{subjectId}").SendAsync("ReceiveMessage", "my app", $"player {playerId} has joined the game{subjectId}.");
        }
        
        //public async Task JoinGameAsync(int playerId, int subjectId, CancellationToken cancellationToken)
        //{
        //    var player = await _playerService.GetByIdAsync(playerId);
        //    var subject = await _subjectService.GetByIdAsync(subjectId);
        //    var game = await _gameService.FindGameAsync(subject, player);

        //    // Add player to game's SignalR group
        //    await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{game.GameID}", cancellationToken);

        //    // Send a message to the group
        //    await Clients.Group($"game_{game.GameID}").SendAsync("ReceiveMessage", "my app", $"{player.PlayerName} has joined the game.");
        //}


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
    }
}
