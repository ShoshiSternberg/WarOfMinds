using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Services.Interfaces;

namespace WarOfMinds.Services.Services
{
    public class PlayerForCalcRating
    {
        public PlayerForCalcRating(PlayerDTO player, double scoreForPlacementPosition, int newEloRating)
        {
            this.player = player;
            this.scoreForPlacementPosition = scoreForPlacementPosition;
            this.newEloRating = newEloRating;
        }
        public PlayerDTO player { get; set; }
        public double probability { get; set; }
        public double scoreForPlacementPosition { get; set; }
        public int newEloRating { get; set; }
    }


    public class EloCalculator : IEloCalculator
    {
        private readonly IConfiguration _EloCalculatorConfiguration;
        private readonly IConfiguration _TriviaHubConfiguration;
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;
        private readonly IDictionary<int, PlayerForCalcRating> _players;
        private readonly int _d;
        private readonly int _Base;
        private readonly int _k;
        public GameService Game { get; set; }
        public EloCalculator(IConfiguration configuration, IGameService gameService, IPlayerService playerService, IDictionary<int, PlayerForCalcRating> players)//צריל לקבל את הערכים או להשלים אותם לבד
        {
            _EloCalculatorConfiguration = configuration.GetSection("EloCalculator");
            _TriviaHubConfiguration = configuration.GetSection("TriviaHub");
            _gameService = gameService;
            _playerService = playerService;
            _d = Convert.ToInt32(_EloCalculatorConfiguration["d"]);//400
            _Base = Convert.ToInt32(_EloCalculatorConfiguration["base"]);//1 בסיס 1 לפונקציה לינארית, גבוה יותר לפונקציה מעריכית
            _k = Convert.ToInt32(_EloCalculatorConfiguration["k"]);//32
            _players = players;
        }

        private async Task Init(int gameID, List<PlayerDTO> playersSortedByScore, List<int> scores)
        {
            GameDTO game = await _gameService.GetByIdInNewScopeAsync(gameID);//זה אמור להיות כל השחקנים מהדאטה בייס
            List<PlayerDTO> playersFromDB = game.Players.ToList<PlayerDTO>();
            for (int i = 0; i < playersFromDB.Count; i++)
            {
                _players.Add(playersFromDB[i].PlayerID, new PlayerForCalcRating(playersFromDB[i], scores[i], 0));
            }
        }


        public async Task UpdateRatingOfAllPlayers(int gameID, List<PlayerDTO> playersSortedByScore, List<int> scores)
        {
            await Init(gameID, playersSortedByScore, scores);
            int NumOfPlayers = _players.Count;
            if (NumOfPlayers < 2)
            {
                return;
            }
            else
            {

                //שלב א- חישוב ההסתברות
                double sumOfPlayersProbabilities = (NumOfPlayers * (NumOfPlayers - 1)) / 2;
                foreach (var player in _players)
                {
                    double currentPlayerWinProbability = 0;
                    foreach (var opponent in _players)
                    {
                        if (player.Key != opponent.Key)
                        {
                            double probabilityToWinAgainstJ = probabilityToWinAgainst(player.Value.player.ELORating, opponent.Value.player.ELORating);
                            currentPlayerWinProbability += probabilityToWinAgainstJ;
                        }
                    }
                    double scaledCurrentPlayerWinProbability = currentPlayerWinProbability / sumOfPlayersProbabilities;

                    player.Value.probability = scaledCurrentPlayerWinProbability;
                }
                //עכשיו יש לכל שחקן שרשום בדאטה בייס שדה במילון שבו רשום את הציון שחזו לו במשחק הזה

                //שלב ב - חישוב הציון בפועל


                //כאן עוברים על המערך של המיקומים
                //לכן אי אפשר לכתוב מספר השחקנים כי יתכן שלא כולם ענו
                double exponentialDivider;
                if (_Base > 1)
                {
                    exponentialDivider = 0;
                    for (int i = 1; i < playersSortedByScore.Count; i++)
                    {
                        exponentialDivider += (Math.Pow(_Base, (playersSortedByScore.Count - i)) - 1);
                    }
                    for (int i = 1; i < playersSortedByScore.Count; i++)
                    {
                        double score = ((Math.Pow(_Base, (playersSortedByScore.Count - i)) - 1)) / exponentialDivider;
                        _players[playersSortedByScore[i].PlayerID].scoreForPlacementPosition = score;
                    }
                }
                else
                {
                    for (int i = 1; i < playersSortedByScore.Count + 1; i++)
                    {
                        double score = (double)(NumOfPlayers - i) / (NumOfPlayers * (NumOfPlayers - 1) / 2);// _players[playersSortedByScore[i].PlayerID].scoreForPlacementPosition / ((NumOfPlayers * _players[playersSortedByScore[i].PlayerID].scoreForPlacementPosition) / 2);                       

                        _players[playersSortedByScore[i - 1].PlayerID].scoreForPlacementPosition = score;

                    }
                }

                //שלב ג - עדכון הדירוג
                foreach (var item in _players)
                {
                    double newEloRating = Math.Round(item.Value.player.ELORating + _k * (NumOfPlayers - 1) * (item.Value.scoreForPlacementPosition - item.Value.probability));
                    item.Value.newEloRating = (int)newEloRating;
                    item.Value.player.ELORating = item.Value.newEloRating;
                    Console.WriteLine($"player: {item.Key} old score: {item.Value.player.ELORating} probability: {item.Value.probability} new score: {item.Value.newEloRating}");
                    await _playerService.UpdatePlayerInNewScopeAsync(item.Value.player);
                }

            }

        }
        public double probabilityToWinAgainst(double opponentRating, double currentPlayerRating)
        {
            return 1 / (1 + Math.Pow(10, (opponentRating - currentPlayerRating) / _d));
        }

    }
}

