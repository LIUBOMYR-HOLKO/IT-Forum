using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IT_Forum.Models.Entities
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public String Title { get; set; }
        [Required]
        public String Context { get; set; }
        
        public String UserId { get; set; }
        public User Creator { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<Like> Likes { get; set; }

        public bool IsMatch(string value)
        {
            return Title.Contains(value);
        }
    }
}
