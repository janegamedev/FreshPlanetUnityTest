using UnityEngine;

namespace FreshPlanet.Data
{
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

        public Song(string id, string title, string artist, string picture, string sample)
        {
            this.id = id;
            this.title = title;
            this.artist = artist;
            this.picture = picture;
            this.sample = sample;
        }
    
        public void SetLoadedPicture(Texture preloadedTexture)
        {
            songPicture = preloadedTexture;
        }
                    
        public void SetLoadedSample(AudioClip preloadedSample)
        {
            songSample = preloadedSample;
        }
    }
}