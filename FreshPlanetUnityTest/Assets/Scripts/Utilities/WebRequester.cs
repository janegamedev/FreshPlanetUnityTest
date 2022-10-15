using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace FreshPlanet.Utilities
{
    public static class WebRequester
    {
        public static Texture lastLoadedTexture;
        public static AudioClip lastLoadedAudioClip;
        
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