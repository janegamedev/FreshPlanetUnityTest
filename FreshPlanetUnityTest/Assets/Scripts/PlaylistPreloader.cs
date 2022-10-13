using System.Collections;
using FreshPlanet.Data;
using FreshPlanet.Utilities;
using UnityEngine;

namespace FreshPlanet
{
    public class PlaylistPreloader : MonoBehaviour
    {
        private Playlist playlist;
        public Playlist Playlist => playlist;
        
        private Coroutine preloadRoutine;

        private void OnDestroy()
        {
            TerminateRoutine();
        }

        private void TerminateRoutine()
        {
            if (preloadRoutine != null)
            {
                StopCoroutine(preloadRoutine);
                preloadRoutine = null;
            }
        }
        
        public void PreloadPlaylist(string id)
        {
            playlist = QuizPlaylists.GetPlaylistById(id);

            if (playlist == null)
            {
                return;
            }

            TerminateRoutine();
            preloadRoutine = StartCoroutine(PlaylistPreloadRoutine());
        }

        private IEnumerator PlaylistPreloadRoutine()
        {
            foreach (Playlist.Question question in playlist.Questions)
            {
                Playlist.Question.Song song = question.CurrentSong;
                if (song.RequiresPicturePreload)
                {
                    yield return WebRequester.RequestTexture(song.PicturePath);
                }
                
                song.SetLoadedPicture(WebRequester.lastLoadedTexture);

                if (song.RequiresSamplePreload)
                {
                    yield return WebRequester.RequestAudioClip(song.SamplePath);
                }
                
                song.SetLoadedSample(WebRequester.lastLoadedAudioClip);
            }

            preloadRoutine = null;
        }
    }
}