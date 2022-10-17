namespace FreshPlanet.Data
{
    public class Result
    {
        private bool answeredCorrectly;
        public bool AnsweredCorrectly => answeredCorrectly;

        private float answerTime;
        public float AnswerTime => answerTime;

        public Result(bool answeredCorrectly, float answerTime)
        {
            this.answeredCorrectly = answeredCorrectly;
            this.answerTime = answerTime;
        }
    }
}