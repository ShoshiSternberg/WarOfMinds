using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Services.Interfaces;

namespace WarOfMinds.Services.Services
{
    //public class EloCalculator : IEloCalculator
    //{
    //    private readonly int _d;
    //    private readonly int _Base;
    //    private readonly int _k;
    //    public GameService Game { get; set; }
    //    public EloCalculator()//צריל לקבל את הערכים או להשלים אותם לבד
    //    {
    //        _d = 400;
    //        _Base = 1;//בסיס 1 לפונקציה לינארית, גבוה יותר לפונקציה מעריכית
    //        _k = 32;
    //        Game = GameService.GetCurrentGame();//איך מקבלים את המשחק הנוכחי
    //    }
        
    //    public async void UpdateRatingOfAllPlayers()
    //    {
    //        List<PlayerDTO> players = await Game.GetAllAsync();
    //        int NumOfPlyers = players.Count;
    //        double sumOfPlayersPropabilities = (NumOfPlyers * (NumOfPlyers - 1)) / 2;
    //        List<double> playersPropabilities = new List<double>();//מערך הסתברויות
    //        List<double> scoreForEachPlacementPosition = new List<double>();//מערך ציונים בפועל לפי מיקום
    //        List<double> newEloRatings = new List<double>();//מערך דירוגים מעודכנים//
    //        //שלב א- חישוב ההסתברות
    //        for (int i = 0; i < players.Count; i++)
    //        {
    //            double currentPlayerWinPropability = 0;
    //            for (int j = 0; j < players.Count; j++)
    //            {
    //                if (i != j)
    //                {
    //                    double opponentRating = players[j].ELORating;
    //                    double propabilityToWinAgainstJ = probabilityToWinAgainst(players[i].ELORating, players[j].ELORating);
    //                    currentPlayerWinPropability += propabilityToWinAgainstJ;
    //                }
    //            }
    //            double scaledCurrentPlayerWinPropability = currentPlayerWinPropability / sumOfPlayersPropabilities;
    //            playersPropabilities.Add(scaledCurrentPlayerWinPropability);


    //        }
    //        //שלב ב - חישוב הציון בפועל
    //        double exponentialDivider;
    //        if (_Base > 1)
    //        {
    //            exponentialDivider = 0;
    //            for (int i = 1; i < players.Count; i++)
    //            {
    //                exponentialDivider += (Math.Pow(_Base, (players.Count - i)) - 1);
    //            }
    //            for (int i = 1; i < players.Count; i++)
    //            {
    //                double score = ((Math.Pow(_Base, (players.Count - i)) - 1)) / exponentialDivider;
    //                scoreForEachPlacementPosition.Add(score);
    //            }

    //        }
    //        else
    //        {
    //            for (int i = 0; i < players.Count; i++)
    //            {
    //                double score = (players.Count - i) / ((players.Count * (players.Count - 1)) / 2);
    //                scoreForEachPlacementPosition.Add(score);
    //            }

    //        }

    //        //שלב ג - עדכון הדירוג
    //        for (int i = 0; i < players.Count; i++)
    //        {
    //            newEloRatings.Add(Math.Round(players[i].ELORating + _k * (players.Count - 1) * (scoreForEachPlacementPosition[i] - playersPropabilities[i])));

    //        }

    //        for (int i = 0; i < players.Count; i++)
    //        {
    //            PlayerDTO playerDTO1 = PlayerService.GetById(players[i].PlayerID);
    //            playerDTO1.ELORating = (int)newEloRatings[i];
    //            await PlayerService.UpdateAsync(playerDTO1);
    //        }

    //    }


    //    public double probabilityToWinAgainst(double opponentRating, double currentPlayerRating)
    //    {
    //        return 1 / (1 + Math.Pow(10, (opponentRating - currentPlayerRating) / _d));
    //    }
    //}
}

