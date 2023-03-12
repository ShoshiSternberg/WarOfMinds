using System.Globalization;
using System.Runtime.Intrinsics.X86;

namespace WarOfMinds.WebApi.SignalR
{
    public class AnswerResult : IComparable<AnswerResult>
    {
        public string player { get; set; }
        public bool Score { get; set; }
        public int AnswerTime { get; set; }

        public int CompareTo(AnswerResult other)
        {
            // First, compare by Score
            if (this.Score != other.Score)
            {
                // Items with Score = true come first
                return this.Score ? -1 : 1;
            }
            else
            {
                return this.AnswerTime.CompareTo(other.AnswerTime);
            }
        }

        public bool IsCorrect(string correct_answer, string playerAnswer)
        {
            return correct_answer == playerAnswer;
        } 
    }

}