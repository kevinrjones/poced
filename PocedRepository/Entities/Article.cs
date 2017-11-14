using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocedRepository.Entities
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

        public byte[] Image { get; set; }
        public byte[] Favicon { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }


}
