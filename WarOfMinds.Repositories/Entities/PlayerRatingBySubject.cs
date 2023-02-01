using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Repositories.Entities
{
    public class PlayerRatingBySubject
    {
        private int playerRatingBySubjectID;

        public int PlayerRatingBySubjectID
        {
            get { return playerRatingBySubjectID; }
            set { playerRatingBySubjectID = value; }
        }

        private Subject subject;

        public Subject Subject
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
