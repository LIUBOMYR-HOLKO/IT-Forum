using System;
using IT_Forum.Models.Entities;

namespace IT_Forum.Models.ViewModel
{
    public class PostViewModel
    {
        public int Id { get; set; }
        
        public String Title { get; set; }
        public String Context { get; set; }
        
        public User Creator { get; set; }
        
        public bool IsLiked { get; set; }
        
        public bool IsHaveAccessToUpdate { get; set; }

        public PostViewModel(Post post, bool isLiked, bool isHaveAccessToUpdate)
        {
            Id = post.PostId;
            Title = post.Title;
            Context = post.Context;
            Creator = post.Creator;
            IsLiked = isLiked;
            IsHaveAccessToUpdate = isHaveAccessToUpdate;
        }
    }
}