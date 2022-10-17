using System.Collections.Generic;

namespace FreshPlanet.Data
{
    public class Question
    {
        private string id;
        public string ID => id;
        
        private int answerIndex;
        public int AnswerIndex => answerIndex;
        
        private List<Choice> choices;
        public List<Choice> Choices => choices;
        
        private Song song;
        public Song CurrentSong => song;

        private string type;

        private Result result;
        public Result Result => result;

        public Question(string id, int answerIndex, List<Choice> choices, Song song, string type)
        {
            this.id = id;
            this.answerIndex = answerIndex;
            this.choices = choices;
            this.song = song;
            this.type = type;
        }

        public void SetResult(Result result)
        {
            this.result = result;
        }
    }
}