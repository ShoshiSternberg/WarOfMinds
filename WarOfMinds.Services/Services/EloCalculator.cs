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
            if (playersSortedByScore.Count == 1)//אם שחקן משחק לבד, אפשר להניח שיש לו יריב וירטואלי עם דרוג זהה לדרוג הקודם שלו
            {
                PlayerDTO p = new PlayerDTO();
                p.PlayerID = 0;
                p.PlayerName = "virtual";
                p.ELORating = playersSortedByScore[0].ELORating;
                PlayerForCalcRating p1 = new PlayerForCalcRating(p, 0, 0);//אם הוא נכשל מול עצמו היריב הוירטואלי מקבל את כל הנקודות- מס השאלות כפול מספר השניות לכל שאלה
                if (scores[0] == 0)
                    p1.scoreForPlacementPosition = Convert.ToInt32(_TriviaHubConfiguration["NumOfQuestions"]) * Convert.ToInt32(_TriviaHubConfiguration["TimeToAnswer"]);
                _players.Add(0, p1);
            }

            GameDTO game = await _gameService.GetWholeByIdAsync(gameID);//זה אמור להיות כל השחקנים מהדאטה בייס
            List<PlayerDTO> playersFromDB = (List<PlayerDTO>)game.Players;
            for (int i = 0; i < playersFromDB.Count; i++)
            {
                _players.Add(playersFromDB[i].PlayerID, new PlayerForCalcRating(playersFromDB[i], scores[i], 0));

            }
        }


        public async void UpdateRatingOfAllPlayers(int gameID, List<PlayerDTO> playersSortedByScore, List<int> scores)
        {
            await Init(gameID, playersSortedByScore, scores);
            int NumOfPlayers = _players.Count;
            if (NumOfPlayers < 2)
            {
                throw (new Exception("There are no players"));

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
                    foreach (var player in _players)
                    {
                        exponentialDivider += (Math.Pow(_Base, player.Value.scoreForPlacementPosition));
                    }
                    foreach (var player in _players)
                    {
                        double score = ((Math.Pow(_Base, player.Value.scoreForPlacementPosition) - 1)) / exponentialDivider;
                        player.Value.scoreForPlacementPosition = score;
                    }
                }
                else
                {
                    foreach (var player in _players)
                    {
                        double score = player.Value.scoreForPlacementPosition / ((NumOfPlayers * player.Value.scoreForPlacementPosition) / 2);
                        player.Value.scoreForPlacementPosition = score;

                    }
                }

                //שלב ג - עדכון הדירוג
                foreach (var item in _players)
                {
                    double newEloRating = Math.Round(item.Value.player.ELORating + _k * (NumOfPlayers - 1) * (item.Value.scoreForPlacementPosition - item.Value.probability));
                    item.Value.player.ELORating = (int)newEloRating;
                    if (item.Value.player.PlayerID != 0)//כמובן שאת הוירטואלי אין צורך לעדכן
                        //await _playerService.UpdateAsync(item.Value.player);
                        Console.WriteLine($"player: {item.Key} old score: {item.Value.player.ELORating} probability: {item.Value.probability} new score: {item.Value.newEloRating}");
                }

            }
        }


        public double probabilityToWinAgainst(double opponentRating, double currentPlayerRating)
        {
            return 1 / (1 + Math.Pow(10, (opponentRating - currentPlayerRating) / _d));
        }
    }
}

