﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Poced.Identity.Shared;

namespace Poced.Repository.Bare.Entities
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
        public virtual PocedUser User { get; set; }
    }


}
