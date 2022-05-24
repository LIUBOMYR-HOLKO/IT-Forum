using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using IT_Forum.Models;
using IT_Forum.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IT_Forum.Controllers
{
    public class LikeController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        
        public LikeController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Like(int postId)
        {
            User user = CurrentUser(User.Identity);
            Like like = new Like
            {
                Post = await _context.Posts.FindAsync(postId),
                PostId = postId,
                User = user,
                UserId = user.Id
            };
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetArticle", "Article", new {id = postId});
        }
        
        [HttpGet]
        public async Task<IActionResult> Dislike(int postId)
        {   
            User user = CurrentUser(User.Identity);
            Like like = _context.Likes.FirstOrDefault(like => like.PostId == postId && like.UserId == user.Id);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("GetArticle", "Article", new {id = postId});
        }
        
        private User CurrentUser(IIdentity user) => _userManager.FindByNameAsync(user.Name).Result;
    }
}