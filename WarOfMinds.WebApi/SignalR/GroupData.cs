using WarOfMinds.Common.DTO;

namespace WarOfMinds.WebApi.SignalR
{
    public class GroupData
    {
        public GameDTO game { get; set; }
        public List<Question> questions { get; set; }
        public Dictionary<int, List<AnswerResult>> gameResults { get; set; }
    }
}
