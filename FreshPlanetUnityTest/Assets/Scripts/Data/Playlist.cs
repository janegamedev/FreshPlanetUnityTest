using System.Collections.Generic;
using UnityEngine;

namespace FreshPlanet.Data
{
    /// <summary>
    /// Playlist data structure that contains an ID, list of questions and the title.
    /// Each question stores its own ID, correct answer index, list of choices and song data.
    /// Song data structure holds all information about specific song in this question: id, title, artist, picture and song sample.
    /// Question choices only contain titles and artists 
    /// </summary>
    public class Playlist
    {
        private string id;
        public string ID => id;

        public class Question
        {
            private string id;
            public string ID => id;
            
            private int answerIndex;
            public int AnswerIndex => answerIndex;

            public class Choice
            {
                private string artist;
                public string Artist => artist;
                
                private string title;
                public string Title => title;
            }
            
            private List<Choice> choices;
            public List<Choice> Choices => choices;

            public class Song
            {
                private string id;
                public string ID => id;
                
                private string title;
                public string Title => title;
                
                private string artist;
                public string Artist => artist;
                
                private string picture;
                public string PicturePath => picture;
                
                private string sample;
                public string SamplePath => sample;
                
                private Texture songPicture;
                public Texture SongPicture => songPicture;
                
                private AudioClip songSample;
                public AudioClip SongSample => songSample;

                public bool RequiresPicturePreload => songPicture == null;
                public bool RequiresSamplePreload => songSample == null;

                public void SetLoadedPicture(Texture preloadedTexture)
                {
                    songPicture = preloadedTexture;
                }
                
                public void SetLoadedSample(AudioClip preloadedSample)
                {
                    songSample = preloadedSample;
                }
            }
            
            private Song song;
            public Song CurrentSong => song;
        }
        
        private List<Question> questions;
        public List<Question> Questions => questions;
        
        private string playlist;
        public string PlaylistTitle => playlist;

        public Playlist(string id, List<Question> questions, string playlist)
        {
            this.id = id;
            this.questions = questions;
            this.playlist = playlist;
        }
    }
}