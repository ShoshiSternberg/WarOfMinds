using WarOfMinds.Common.DTO;

namespace WarOfMinds.WebApi.SignalR
{
    public class GroupData
    {
        public GameDTO game { get; set; }
        public bool IsActive { get; set; }
        public string GameManagerConnectionID { get; set; }
        public List<Question> questions { get; set; }
        public List<AnswerResult> gameResults { get; set; }//ליסט של תשובות- הוא מתרוקן בסיום כל שאלה.
        public bool IsTimeOver { get; set; }
        public int RightAnswers { get; set; } = 0;
    }
}
