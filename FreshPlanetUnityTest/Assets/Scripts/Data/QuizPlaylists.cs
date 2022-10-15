using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace FreshPlanet.Data
{
    /// <summary>
    /// Data class that stores all playlists loaded from the json data file
    /// Loads playlists data upon creation and stores them both in the list and (id, playlist) dictionary
    /// </summary>
    public class QuizPlaylists
    {
        private const string DATA_JSON_PATH = "Assets/Resources/coding-test-frontend-unity.json";

        public static List<Playlist> Playlists { get; private set; } = new List<Playlist>();
        private static Dictionary<string, Playlist> playlistsTable = new Dictionary<string, Playlist>();

        static QuizPlaylists()
        {
            LoadPlaylistData();
        }
        
        /// <summary>
        /// Loads playlists data from json at the DATA_JSON_PATH path
        /// Deserializes it using JsonConvert to list of playlists
        /// </summary>
#if UNITY_EDITOR
        [MenuItem("FreshPlanet/Playlists/Refresh Playlists")]
#endif
        public static void LoadPlaylistData()
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

        public static Playlist GetPlaylistById(string id)
        {
            if (playlistsTable.TryGetValue(id, out Playlist playlist))
            {
                return playlist;
            }

            return null;
        }
    }
}