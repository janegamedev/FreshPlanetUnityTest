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

        public Question(string id, int index, List<Choice> choices, Song song, string type)
        {
            this.id = id;
            answerIndex = index;
            this.choices = choices;
            this.song = song;
            this.type = type;
        }
    }
}