using System;
using System.Collections;
using FreshPlanet.Data;
using FreshPlanet.Utilities;
using UnityEngine;

namespace FreshPlanet
{
    /// <summary>
    /// Preloads the specific preloadedPlaylist by ID and stores it until another preloadedPlaylist is requested
    /// </summary>
    public class PlaylistPreloader : Singleton<PlaylistPreloader>
    {
        public static event Action<PlaylistPreloader, Playlist> OnPlaylistPreloadCompleted;

        public Playlist PreloadedPlaylist { get; private set; }

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
        /// Preloads the given preloadedPlaylist
        /// Starts the preloading routine that will invoke OnPlaylistPreloadCompleted on completion
        /// </summary>
        /// <param name="playlist">PreloadedPlaylist to preload</param>
        public void PreloadPlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                return;
            }

            TerminateRoutine();
            preloadRoutine = StartCoroutine(PlaylistPreloadRoutine(playlist));
        }

        /// <summary>
        /// PreloadedPlaylist preload routine
        /// Iterates questions in the given preloadedPlaylist and loads song picture and sample if required
        /// Invokes OnPlaylistPreloadCompleted event upon completion
        /// </summary>
        private IEnumerator PlaylistPreloadRoutine(Playlist playlistPreload)
        {
            PreloadedPlaylist = playlistPreload;
            
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