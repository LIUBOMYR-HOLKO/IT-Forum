using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IT_Forum.Models.Entities
{
    public class Comment
    {
        [Required]
        public String BodyOfComment { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public String UserId { get; set; }
        public User User { get; set; }
    }
}
