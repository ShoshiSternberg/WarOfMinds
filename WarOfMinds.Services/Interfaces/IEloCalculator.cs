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
        public void UpdateRatingOfAllPlayers();//עדכון דירוג כל השחקנים
    }
}
