using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Repositories.Entities
{
    public class Subject
    {
        private int subjectID;

        public int SubjectID
        {
            get { return subjectID; }
            set { subjectID = value; }
        }


        private string subjectName;

        public string Subjectname
        {
            get { return subjectName; }
            set { subjectName = value; }
        }

        private int difficultyLevel;

        public int DifficultyLevel
        {
            get { return difficultyLevel; }
            set { difficultyLevel = value; }
        }


    }
}
