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
        public int GameId { get; set; }
        public Game PGame { get; set; }
        public int PlayerId { get; set; }
        public Player GPlayer { get; set; }
    }
}
