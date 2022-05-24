using IT_Forum.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace IT_Forum.Models
{
    public class Context : IdentityDbContext<User>
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
        
        public new DbSet<User> Users { get; set; }
        public new DbSet<Post> Posts { get; set; }
        public new DbSet<Like> Likes { get; set; }
        public new DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasOne(post => post.Creator)
                .WithMany(user => user.OneToManyPosts)
                .HasForeignKey(post => post.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}