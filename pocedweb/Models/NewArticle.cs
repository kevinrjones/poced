using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pocedweb.Models
{
    public class NewArticle
    {
        public NewArticle()
        {
            Tags = new List<string>();
        }

        [Required(ErrorMessage = "Title is Required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Url is Required")]
        public string Url { get; set; }

        public List<string> Tags { get; set; }
    }
}