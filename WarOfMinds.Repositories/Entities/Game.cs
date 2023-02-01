using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Repositories.Entities
{
    public class Game
    {
        private int gameID;

        public int GameID
        {
            get { return gameID; }
            set { gameID = value; }
        }

        private Subject subject;

        public Subject Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        private DateTime gameDate;

        public DateTime GameDate
        {
            get { return gameDate; }
            set { gameDate = value; }
        }


        private int gameLength;

        public int GameLength
        {
            get { return gameLength; }
            set { gameLength = value; }
        }

        private Player gameManager;

        public Player GameManager
        {
            get { return gameManager; }
            set { gameManager = value; }
        }
         
        public virtual ICollection<Player> Players { get; set; }
    }
}
