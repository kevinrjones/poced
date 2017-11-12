using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pocedweb.Models
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