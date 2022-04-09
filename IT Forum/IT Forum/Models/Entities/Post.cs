using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IT_Forum.Models.Entities
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public String Title { get; set; }
        public String Description { get; set; }
        [Required]
        public String BodyOfPost { get; set; }

        public String UserId { get; set; }
        public User User { get; set; }

        public ICollection<User> Users { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }

        public bool IsMatch(string value)
        {
            return Title.Contains(value);
        }
    }
}
