using Microsoft.AspNetCore.SignalR;

namespace WarOfMinds.WebApi.SignalR
{
    public class TriviaHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            // Add the client to the list of connected clients
            _connectedClients.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Remove the client from the list of connected clients
            _connectedClients.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task ReceiveAnswer(string answer)
        {
            // Process the answer and update the game state
            // ...

            // Send the updated game state to the clients
            await SendMessage("The game state has been updated");
        }
    }
}
