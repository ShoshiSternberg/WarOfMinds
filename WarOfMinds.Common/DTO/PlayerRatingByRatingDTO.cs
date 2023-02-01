using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Common.DTO
{
    public class PlayerRatingBySubjectDTO
    {
        private int playerRatingBySubjectID;

        public int PlayerRatingBySubjectID
        {
            get { return playerRatingBySubjectID; }
            set { playerRatingBySubjectID = value; }
        }

        private int subject;

        public int Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        private int eloRating;

        public int ELORating
        {
            get { return eloRating; }
            set { eloRating = value; }
        }

    }
}
