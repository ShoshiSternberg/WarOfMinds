using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Repositories.Entities
{
    public class Player
    {
        public Player()
        {
            this.Games = new HashSet<GamePlayer>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        private string playerEmail;

        public string PlayerEmail
        {
            get { return playerEmail; }
            set { playerEmail = value; }
        }

        private DateTime dateOfRegistration;

        public DateTime DateOfRegistration
        {
            get { return dateOfRegistration; }
            set { dateOfRegistration = value; }
        }

        private string playerAddress;

        public string PlayerAddress
        {
            get { return playerAddress; }
            set { playerAddress = value; }
        }


        private int eloRating;

        public int ELORating
        {
            get { return eloRating; }
            set { eloRating = value; }
        }

        public virtual ICollection<GamePlayer> Games { get; set; }
    }
}
