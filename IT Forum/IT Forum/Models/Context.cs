using IT_Forum.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IT_Forum.Models
{
    public class Context : IdentityDbContext<User>
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
        
        public new DbSet<User> Users;
        public DbSet<Post> Posts;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasOne(post => post.Creator)
                .WithMany(user => user.OneToManyPosts)
                .HasForeignKey(post => post.UserId);

            modelBuilder.Entity<Post>()
                .HasMany(post => post.Users)
                .WithMany(user => user.ManyToManyPosts)
                .UsingEntity<Like>(
                    j => j
                        .HasOne(like => like.User)
                        .WithMany(user => user.Likes)
                        .HasForeignKey(like => like.UserId),
                    j => j
                        .HasOne(like => like.Post)
                        .WithMany(post => post.Likes)
                        .HasForeignKey(like => like.PostId),
                    j =>
                    {
                        j.HasKey(user => new { user.PostId, user.UserId });
                    });

            modelBuilder.Entity<Post>()
                .HasMany(post => post.Users)
                .WithMany(user => user.ManyToManyPosts)
                .UsingEntity<Comment>(
                    j => j
                        .HasOne(comment => comment.User)
                        .WithMany(user => user.Comments)
                        .HasForeignKey(comment => comment.UserId),
                    j => j
                        .HasOne(comment => comment.Post)
                        .WithMany(post => post.Comments)
                        .HasForeignKey(comment => comment.PostId),
                    j =>
                    {
                        j.Property(comment => comment.BodyOfComment);
                        j.HasKey(user => new { user.PostId, user.UserId });
                    });

            base.OnModelCreating(modelBuilder);
        }
    }
}