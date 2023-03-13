using WarOfMinds.Common.DTO;

namespace WarOfMinds.WebApi.SignalR
{
    public class UserConnection
    {
        public UserConnection(PlayerDTO player, int game)
        {
            this.game = game;
            this.player = player;
        }
        public PlayerDTO player { get; set; }
        public int game { get; set; }
    }
}
