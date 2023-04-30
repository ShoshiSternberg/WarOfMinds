using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfMinds.Repositories.Entities
{
    public class GamePlayer
    {
        [Key]
        public Game Game { get; set; }
        [Key] 
        public Player Player { get; set; }
    }
}
