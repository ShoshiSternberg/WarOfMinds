using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Common.DTO
{
    public class PlayerDTO
    {
        private int playerID;

        public int PlayerID
        {
            get { return playerID; }
            set { playerID = value; }
        }

        private string playerName;

        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        private string playerPassword;

        public string PlayerPassword
        {
            get { return playerPassword; }
            set { playerPassword = value; }
        }

        private DateTime dateOfRegistration;

        public DateTime DateOfRegistration
        {
            get { return dateOfRegistration; }
            set { dateOfRegistration = value; }
        }

        private int eloRating;

        public int ELORating
        {
            get { return eloRating; }
            set { eloRating = value; }
        }

        public virtual ICollection<GameDTO> Games { get; set; }

    }
}
