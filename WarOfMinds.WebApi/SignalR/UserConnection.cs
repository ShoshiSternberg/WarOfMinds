using WarOfMinds.Common.DTO;

namespace WarOfMinds.WebApi.SignalR
{
    public class UserConnection
    {
        public PlayerDTO player { get; set; }
        public GameDTO game { get; set; }
    }
}
