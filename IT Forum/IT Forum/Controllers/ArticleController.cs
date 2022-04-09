using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using IT_Forum.Models;
using IT_Forum.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IT_Forum.Helpers;
using IT_Forum.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IT_Forum.Controllers
{
    public class ArticleController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        
        public ArticleController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetArticles(
            [FromQuery] PaginationFilter filter,[FromQuery]string sortBy, [FromQuery]string search, [FromQuery]string order="asc")
        {
            sortBy = string.IsNullOrEmpty(sortBy) ? "Id" : sortBy;
            var sortByProperty = typeof(Post).GetProperty(sortBy);
            
            var resultList = new List<Post>();

            if (!string.IsNullOrEmpty(search))
            {
                await foreach (var post in _context.Posts)
                {
                    if (post.IsMatch(search))
                    {
                        resultList.Add(post);
                    }
                }
            }
            else
            {
                resultList = _context.Posts.ToList();
            }

            filter.ToValidOptions(resultList.Count());
            
            var pagedData = resultList
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize).ToList().OrderBy(article =>
                    sortByProperty?.GetValue(article)).ToList();
            if (order == "desc")
            {
                pagedData.Reverse();
            }
            // return Articles
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            if (!IsArticleExist(id))
            {
                // Return error
            }

            Post article = await _context.Posts.FindAsync(id);
            // Return article
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            // Return post back
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, Post post)
        {
            if (!IsArticleExist(id))
            {
                // Return error
            }
            
            User currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);
            Post article = await _context.Posts.FindAsync(id);
            if (!currentUser.OneToManyPosts.Contains(article) && !currentUser.IsAdmin)
            {
                // Return error
            }

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return article
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            if (!IsArticleExist(id))
            {
                // Return error
            }
            
            User currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);
            Post article = await _context.Posts.FindAsync(id);
            if (!currentUser.OneToManyPosts.Contains(article) && !currentUser.IsAdmin)
            {
                // Return error
            }
            _context.Posts.Remove(article);
            await _context.SaveChangesAsync();
        }

        private bool IsArticleExist(int id) => _context.Posts.Any(e => e.PostId == id);
        
        private bool IsUserAdmin(IIdentity user) => _userManager.FindByNameAsync(user.Name).Result.IsAdmin;

    }
}