using System.Globalization;
using System.Runtime.Intrinsics.X86;
using WarOfMinds.Common.DTO;

namespace WarOfMinds.WebApi.SignalR
{
    public class AnswerResult : IComparable<AnswerResult>
    {

        public string connectionId { get; set; }
        public PlayerDTO player { get; set; }
        //public int Score { get; set; } לא בטוח שצריך את זה
        public int AnswerTime { get; set; }
        public bool answer { get; set; }
        public int CompareTo(AnswerResult other)
        {
            // First, compare by Score
            //if (this.answer != other.answer)
            //{
            //    // Items with Score = true come first
            //    return this.answer ? -1 : 1;
            //}
            //else
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