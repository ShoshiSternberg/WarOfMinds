namespace WarOfMinds.WebApi.SignalR
{
    public class AnswerResult
    {
        public int Score { get; set; }
        public TimeSpan AnswerTime { get; set; }

        public bool IsCorrect(int answerId, int playerAnswer)
        {
            return answerId== playerAnswer;
        }
    }
}