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

        public PostViewModel(Post post, User user = null)
        {
            Id = post.PostId;
            Title = post.Title;
            Context = post.Context;
            Creator = post.Creator;
            IsLiked = user != null && post.Likes != null && post.Likes.Exists(like => like.PostId == post.PostId && like.UserId == user.Id);
        }
    }
}