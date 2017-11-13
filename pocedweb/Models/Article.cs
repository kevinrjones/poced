using System.Collections.Generic;

namespace PocedWeb.Models
{
    public class Article
    {
        public Article()
        {
            Tags = new List<string>();

        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public List<string> Tags { get; set; }
    }
}