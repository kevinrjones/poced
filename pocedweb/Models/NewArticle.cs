using System.Collections.Generic;

namespace pocedweb.Models
{
    public class NewArticle
    {
        public NewArticle()
        {
            Tags = new List<string>();
        }
        public string Title { get; set; }
        public string Url { get; set; }

        public List<string> Tags { get; set; }
    }
}