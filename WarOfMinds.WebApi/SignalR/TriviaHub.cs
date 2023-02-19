using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client.Extensibility;
using System;
using System.Collections.Generic;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Services.Services;

namespace WarOfMinds.WebApi.SignalR
{

    public class TriviaHub : Hub
    {
        private readonly GameService _gameService;
        private readonly PlayerService _playerService;
        private readonly SubjectService _subjectService;

        public TriviaHub(GameService gameService, PlayerService playerService, SubjectService subjectService)
        {
            _gameService = gameService;
            _playerService = playerService;
            _subjectService = subjectService;
        }

        public async Task JoinGameAsync(int playerId, int subjectId, CancellationToken cancellationToken)
        {
            var player = await _playerService.GetByIdAsync(playerId);
            var subject = await _subjectService.GetByIdAsync(subjectId);
            var game = await _gameService.FindGameAsync(subject, player);

            // Add player to game's SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"game_{game.GameID}", cancellationToken);

            // Send a message to the group
            await Clients.Group($"game_{game.GameID}").SendAsync("ReceiveMessage", "my app", $"{player.PlayerName} has joined the game.");
        }


        //להמשיך ולשאול את GPT בקשר ללולאה עם הטיימר
        public async Task GameProgressAsync(int gameId)
        {
            // Get the game by ID
            GameDTO game =await _gameService.GetByIdAsync(gameId);

            if (game == null)
            {
                // Game not found, handle error or return
                return;
            }

            // Set up the game loop
            var questions = game.Subject.Questions;
            var scores = new Dictionary<PlayerDTO, List<AnswerResult>>();

            for (int i = 0; i < questions.Count; i++)
            {
                // Send question to players in the group
                await Clients.Group(gameId).SendAsync("ReceiveQuestion", questions[i]);

                // Wait for players to answer
                var timeToAnswer = TimeSpan.FromSeconds(30);
                var answers = new List<PlayerAnswer>();
                var timer = new Timer(async _ =>
                {
                    // Get answers and calculate scores
                    answers = await _dbContext.PlayerAnswers.Include(pa => pa.Player)
                                                             .Where(pa => pa.GameID == game.GameID && pa.QuestionID == questions[i].QuestionID)
                                                             .ToListAsync();

                    foreach (var answer in answers)
                    {
                        if (!scores.ContainsKey(answer.Player))
                        {
                            scores[answer.Player] = new List<AnswerResult>();
                        }

                        scores[answer.Player].Add(new AnswerResult
                        {
                            IsCorrect = answer.IsCorrect,
                            TimeToAnswer = answer.TimeToAnswer
                        });
                    }
                }, null, timeToAnswer, System.Threading.Timeout.InfiniteTimeSpan);

                // Wait for players to answer
                await Task.Delay(timeToAnswer);

                // Cancel the timer and get the final list of answers
                timer.Dispose();

                // Send answers to players in the group
                await Clients.Group(gameId).SendAsync("ReceiveAnswers", answers);
            }

            // End of game, calculate winner
            var winner = scores.OrderByDescending(s => s.Value.Sum(a => a.IsCorrect ? a.TimeToAnswer : 0)).FirstOrDefault();

            if (winner != null)
            {
                // Send results to players in the group
                await Clients.Group(gameId).SendAsync("GameResults", scores, winner.Key);
            }

            // Remove players from the group for the game
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }
    }
}
