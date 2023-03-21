using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;
using WarOfMinds.Services.Interfaces;

namespace WarOfMinds.Services.Services
{
    public class PlayerForCalcRating
    {
        public PlayerForCalcRating(PlayerDTO player,double scoreForPlacementPosition,int newEloRating)
        {
            this.player = player;
            this.scoreForPlacementPosition= scoreForPlacementPosition;
            this.newEloRating = newEloRating;
        }
        public PlayerDTO player { get; set; }
        public double probability { get; set; }
        public double scoreForPlacementPosition { get; set; }
        public int newEloRating { get; set; }
    }


    public class EloCalculator : IEloCalculator
    {
        private readonly IConfiguration _configuration;
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;
        private readonly IDictionary<int, PlayerForCalcRating> _players;
        private readonly int _d;
        private readonly int _Base;
        private readonly int _k;
        public GameService Game { get; set; }
        public EloCalculator(IConfiguration configuration, IGameService gameService, IPlayerService playerService, IDictionary<int, PlayerForCalcRating> players)//צריל לקבל את הערכים או להשלים אותם לבד
        {
            _configuration = configuration.GetSection("EloCalculator");
            _gameService = gameService;
            _playerService = playerService;
            _d = Convert.ToInt32(_configuration["d"]);//400
            _Base = Convert.ToInt32(_configuration["base"]);//1 בסיס 1 לפונקציה לינארית, גבוה יותר לפונקציה מעריכית
            _k = Convert.ToInt32(_configuration["k"]);//32
            _players = players;
        }

        private void Init(int gameID, List<PlayerDTO> playersSortedByScore)
        {
            List<PlayerDTO> playersFromDB = _gameService.GetByIdAsync(gameID).Result.Players.ToList<PlayerDTO>(); ;//זה אמור להיות כל השחקנים מהדאטה בייס

            List<PlayerForCalcRating> players = new List<PlayerForCalcRating>();
            for (int i = 0; i < playersFromDB.Count; i++)
            {
                _players.Add(playersFromDB[i].PlayerID, new PlayerForCalcRating(playersFromDB[i],0,0));

            }
        }


        public async void UpdateRatingOfAllPlayers(int gameID, List<PlayerDTO> playersSortedByScore)
        {
            Init(gameID, playersSortedByScore);
            int NumOfPlayers = _players.Count;


            //שלב א- חישוב ההסתברות
            double sumOfPlayersPropabilities = (NumOfPlayers * (NumOfPlayers - 1)) / 2;
            foreach (var player in _players)
            {
                double currentPlayerWinPropability = 0;
                foreach (var opponent in _players)
                {
                    if (player.Key != opponent.Key)
                    {
                        double propabilityToWinAgainstJ = probabilityToWinAgainst(player.Value.player.ELORating, opponent.Value.player.ELORating);
                        currentPlayerWinPropability += propabilityToWinAgainstJ;
                    }
                }
                double scaledCurrentPlayerWinPropability = currentPlayerWinPropability / sumOfPlayersPropabilities;

                player.Value.probability = scaledCurrentPlayerWinPropability;
            }
            //עכשיו יש לכל שחקן שרשום בדאטה בייס שדה במילון שבו רשום את הציון שחזו לו במשחק הזה

            //שלב ב - חישוב הציון בפועל
            double exponentialDivider;           
            
           //כאן עוברים על המערך של המיקומים
           //לכן אי אפשר לכתוב מספר השחקנים כי יתכן שלא כולם ענו
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
                for (int i = 0; i < playersSortedByScore.Count; i++)
                {
                    double score = (playersSortedByScore.Count - i) / ((playersSortedByScore.Count * (playersSortedByScore.Count - 1)) / 2);
                    _players[playersSortedByScore[i].PlayerID].scoreForPlacementPosition = score;

                }
            }

            //שלב ג - עדכון הדירוג
            foreach (var item in _players)
            {
                double newEloRating = Math.Round(item.Value.player.ELORating + _k * (NumOfPlayers - 1) * (item.Value.scoreForPlacementPosition - item.Value.probability));
                item.Value.player.ELORating = (int)newEloRating;
                await _playerService.UpdateAsync(item.Value.player);
            }

        }


        public double probabilityToWinAgainst(double opponentRating, double currentPlayerRating)
        {
            return 1 / (1 + Math.Pow(10, (opponentRating - currentPlayerRating) / _d));
        }
    }
}

