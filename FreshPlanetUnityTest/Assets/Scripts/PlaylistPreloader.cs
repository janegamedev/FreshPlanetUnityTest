﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FreshPlanet.Data;
using FreshPlanet.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace FreshPlanet
{
    /// <summary>
    /// Stores all playlists loaded from the json data file
    /// Loads playlists data upon creation and stores them both in the list and (id, playlist) dictionary
    /// Preloads the specific preloadedPlaylist by ID and stores it until another preloadedPlaylist is requested
    /// </summary>
    public class PlaylistPreloader : Singleton<PlaylistPreloader>
    {
        private const string DATA_JSON_PATH = "Assets/Resources/coding-test-frontend-unity.json";

        public static event Action<PlaylistPreloader, Playlist> OnPlaylistPreloadCompleted;

        public List<Playlist> Playlists { get; private set; } = new List<Playlist>();
        private readonly Dictionary<string, Playlist> playlistsTable = new Dictionary<string, Playlist>();
        
        public Playlist PreloadedPlaylist { get; private set; }

        private Coroutine preloadRoutine;

        protected override void Awake()
        {
            base.Awake();
            LoadPlaylistData();
        }

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
        /// Loads playlists data from json at the DATA_JSON_PATH path
        /// Deserializes it using JsonConvert to list of playlists
        /// </summary>
        private void LoadPlaylistData()
        {
            string jsonString = File.ReadAllText(DATA_JSON_PATH);
            Playlists = JsonConvert.DeserializeObject<List<Playlist>>(jsonString);

            playlistsTable.Clear();
            foreach (Playlist playlist in Playlists)
            {
                if (playlistsTable.ContainsKey(playlist.ID))
                {
                    Debug.LogWarning($"Duplicated playlist key {playlist.ID} found in {playlist.PlaylistTitle} playlist");
                    continue;
                }
                
                playlistsTable.Add(playlist.ID, playlist);
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
            
            foreach (Question question in playlistPreload.Questions)
            {
                Song song = question.CurrentSong;
                
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