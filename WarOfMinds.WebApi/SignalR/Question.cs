namespace WarOfMinds.WebApi.SignalR
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Question
    {
        public int questionId { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public string difficulty { get; set; }
        public string question { get; set; }
        public string correct_answer { get; set; }
        public List<string> incorrect_answers { get; set; }
    }

    public class Root
    {
        public int response_code { get; set; }
        public List<Question> results { get; set; }
    }

}
