using System;
using System.Collections;
using FreshPlanet.Data;
using FreshPlanet.Utilities;
using UnityEngine;

namespace FreshPlanet
{
    /// <summary>
    /// Preloads the specific playlist by ID and stores it until another playlist is requested
    /// </summary>
    public class PlaylistPreloader : MonoBehaviour
    {
        public static event Action<PlaylistPreloader, Playlist> OnPlaylistPreloadCompleted;
        
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
        
        /// <summary>
        /// Preloads playlist with the given id
        /// Starts the preloading routine that will invoke OnPlaylistPreloadCompleted on completion
        /// </summary>
        /// <param name="id">Playlist ID to preload</param>
        public void PreloadPlaylist(string id)
        {
            playlist = QuizPlaylists.GetPlaylistById(id);

            if (playlist == null)
            {
                return;
            }

            TerminateRoutine();
            preloadRoutine = StartCoroutine(PlaylistPreloadRoutine(playlist));
        }

        /// <summary>
        /// Playlist preload routine
        /// Iterates questions in the given playlist and loads song picture and sample if required
        /// Invokes OnPlaylistPreloadCompleted event upon completion
        /// </summary>
        private IEnumerator PlaylistPreloadRoutine(Playlist playlistPreload)
        {
            foreach (Playlist.Question question in playlistPreload.Questions)
            {
                Playlist.Question.Song song = question.CurrentSong;
                
                if (song.RequiresPicturePreload)
                {
                    yield return WebRequester.RequestTexture(song.PicturePath);
                    song.SetLoadedPicture(WebRequester.lastLoadedTexture);
                }
                
                if (song.RequiresSamplePreload)
                {
                    yield return WebRequester.RequestAudioClip(song.SamplePath);
                    song.SetLoadedSample(WebRequester.lastLoadedAudioClip);
                }
            }

            OnPlaylistPreloadCompleted?.Invoke(this, playlistPreload);
            preloadRoutine = null;
        }
    }
}