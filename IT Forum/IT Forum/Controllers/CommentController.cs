using System.Security.Principal;
using System.Threading.Tasks;
using IT_Forum.Models;
using IT_Forum.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IT_Forum.Controllers
{
    public class CommentController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        
        public CommentController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Comment(int postId, string content)
        {
            User user = CurrentUser(User.Identity);

            Comment comment = new Comment()
            {
                Post = await _context.Posts.FindAsync(postId),
                PostId = postId,
                Context = content,
                User = user,
                UserId = user.Id
            };
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetArticle", "Article", new {id = postId});
        }

        private User CurrentUser(IIdentity user) => _userManager.FindByNameAsync(user.Name).Result;

    }
}