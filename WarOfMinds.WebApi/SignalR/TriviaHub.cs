using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client.Extensibility;
using WarOfMinds.Services.Services;

namespace WarOfMinds.WebApi.SignalR
{

    public class TriviaHub : Hub
    {
        private readonly GameService _gameService;
        private readonly PlayerService _playerService;
        private readonly SubjectService _subjectService;

        public TriviaHub(GameService gameService, PlayerService playerService, SubjectService subjectService)
        {
            _gameService = gameService;
            _playerService = playerService;
            _subjectService = subjectService;   
        }

        public async Task JoinGroupAsync(int playerID,string subject)
        {
            UserConnection currentConnection=new UserConnection();
            currentConnection.player= _playerService.GetByIdAsync(playerID).Result;
            currentConnection.game=_gameService.GetActiveGameBySubjectAndRatingAsync(subjectID,currentConnection.player.ELORating).Result;
            string groupName = "gameId: " + currentConnection.game.GameID;
            
            currentConnection.game = _gameService.FindGameAsync(subject, currentConnection.player.ELORating).Result;
            _gameService.UpdateRating(currentConnection.game,currentConnection.player.ELORating);
            this.Groups.AddToGroupAsync(this.Context.ConnectionId,"gameId: "+ currentConnection.game.GameID);
        }


    }
}
