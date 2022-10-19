using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace FreshPlanet.Utilities
{
    /// <summary>
    /// Web requester class contains logic for requesting texture and audio clips through the UnityWebRequest
    /// Stores last loaded texture and audio clip at static variables
    /// </summary>
    public static class WebRequester
    {
        public static Texture lastLoadedTexture;
        public static AudioClip lastLoadedAudioClip;
        
        /// <summary>
        /// Requests the texture from the given path through the UnityWebRequest
        /// Stores the result in WebRequester.lastLoadedTexture
        /// </summary>
        /// <param name="path">Path to request the texture from</param>
        public static IEnumerator RequestTexture(string path) 
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
            
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.Log(www.error);
                lastLoadedTexture = null;
            }
            else 
            {
                lastLoadedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }
        
        /// <summary>
        /// Requests the audio clip (in WAV format) from the given path through the UnityWebRequest
        /// Stores the result in WebRequester.lastLoadedAudioClip
        /// </summary>
        /// <param name="path">Path to request the audio clip from</param>
        public static IEnumerator RequestAudioClip(string path)
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
            
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.Log(www.error);
                lastLoadedAudioClip = null;
            }
            else 
            {
                lastLoadedAudioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
            }
        }
    }
}