using System;
using System.Collections.Generic;
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
        
        public List<Comment> Comments { get; set; }

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }

        public PostViewModel(Post post, bool isLiked, bool isHaveAccessToUpdate, List<Comment> comments)
        {
            Id = post.PostId;
            Title = post.Title;
            Context = post.Context;
            Creator = post.Creator;
            IsLiked = isLiked;
            IsHaveAccessToUpdate = isHaveAccessToUpdate;
            Comments = comments;
        }

        public PostViewModel(Post post, int likes, int coments)
        {
            Id = post.PostId;
            Title = post.Title;
            Context = post.Context;
            Creator = post.Creator;
            LikeCount = likes;
            CommentCount = coments;
        }
    }
}