using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Common.DTO
{
    public class GameDTO
    {
        private int gameID;

        public int GameID
        {
            get { return gameID; }
            set { gameID = value; }
        }

        private SubjectDTO subject;

        public SubjectDTO Subject
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

        private PlayerDTO gameManager;

        public PlayerDTO GameManager
        {
            get { return gameManager; }
            set { gameManager = value; }
        }

        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        private int rating;

        public int Rating
        {
            get { return rating; }
            set { rating = value; }
        }

        public virtual ICollection<PlayerDTO> Players { get; set; }

    }
}
