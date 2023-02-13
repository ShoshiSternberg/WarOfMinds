using Microsoft.AspNetCore.SignalR;

namespace WarOfMinds.WebApi.SignalR
{

    public class TriviaHub : Hub
    {

        public void JoinGroup(string groupName)
        {
            this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
        }


    }
}
