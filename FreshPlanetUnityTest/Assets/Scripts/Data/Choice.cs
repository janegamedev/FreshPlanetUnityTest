namespace FreshPlanet.Data
{
    public class Choice
    {
        private string artist;
        public string Artist => artist;
                    
        private string title;
        public string Title => title;

        public Choice(string artist, string title)
        {
            this.artist = artist;
            this.title = title;
        }
    }
}