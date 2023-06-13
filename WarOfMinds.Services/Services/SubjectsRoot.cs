using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Services.Services
{
    public class SubjectsRoot
    {
        public List<subjectAPI> trivia_categories { get; set; }
    }

    public class subjectAPI
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
