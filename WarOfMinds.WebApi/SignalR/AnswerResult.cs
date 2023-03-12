namespace WarOfMinds.WebApi.SignalR
{
    public class AnswerResult
    {
        public string player { get; set; }
        public bool Score { get; set; }
        public int AnswerTime { get; set; }

        public bool IsCorrect(string correct_answer, string playerAnswer)
        {
            return correct_answer == playerAnswer;
        }
    }
}