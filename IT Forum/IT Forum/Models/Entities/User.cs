using IT_Forum.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IT_Forum.Models
{
    public class User : IdentityUser
    {
        public bool IsAdmin { get; set; }
        
        public List<Post> OneToManyPosts { get; set; }
        public ICollection<Post> ManyToManyPosts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
    }
}
