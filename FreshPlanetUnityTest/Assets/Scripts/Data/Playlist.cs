using System.Collections.Generic;
using UnityEngine;

namespace FreshPlanet.Data
{
    public enum PlaylistStatus
    {
        Mastered,
        Activated
    }
    
    /// <summary>
    /// PreloadedPlaylist data structure that contains an ID, list of questions and the title.
    /// Each question stores its own ID, correct answer index, list of choices and song data.
    /// Song data structure holds all information about specific song in this question: id, title, artist, picture and song sample.
    /// Question choices only contain titles and artists 
    /// </summary>
    public class Playlist
    {
        private const string PLAYERPREFS_PLAYLIST_COMPLETED_QUESTIONS_KEY = "PlaylistCompleted_{0}";
        
        private string id;
        public string ID => id;
        
        private List<Question> questions = new List<Question>();
        public List<Question> Questions => questions;
        
        private string playlist;
        public string PlaylistTitle => playlist;

        public Playlist(string id, List<Question> questions, string playlist)
        {
            this.id = id;
            this.questions = questions;
            this.playlist = playlist;
        }

        public int GetMaxCompletedQuestions()
        {
            return PlayerPrefs.GetInt(string.Format(PLAYERPREFS_PLAYLIST_COMPLETED_QUESTIONS_KEY, id), 0);
        }

        public void SetCompletedQuestionsAmount(int completed)
        {
            PlayerPrefs.SetInt(string.Format(PLAYERPREFS_PLAYLIST_COMPLETED_QUESTIONS_KEY, id), Mathf.Min(completed, Questions.Count));
        }

        public PlaylistStatus GetPlaylistStatus()
        {
            return GetMaxCompletedQuestions() >= Questions.Count ? PlaylistStatus.Mastered : PlaylistStatus.Activated;
        }
    }
}