using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;

namespace WarOfMinds.Services.Interfaces
{
    public interface IEloCalculator
    {
        public Task UpdateRatingOfAllPlayers(int gameID,List<PlayerDTO> playersSortedByScore, List<int> scors);//עדכון דירוג כל השחקנים
    }
}
