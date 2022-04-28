using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using IT_Forum.Models;
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

            filter.ToValidOptions(resultList.Count);
            
            var pagedData = resultList
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize).ToList().OrderBy(article =>
                    sortByProperty?.GetValue(article)).ToList();
            if (order == "desc")
            {
                pagedData.Reverse();
            }
            var totalRecords = await _context.Posts.CountAsync();
            var pagedResponse = PaginationHelper.CreatePagedResponse(pagedData, filter, totalRecords);
            
            if (pagedResponse.TotalRecords == 0)
            {
                pagedResponse.Message = "Page is empty";
            }

            return View(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            if (!IsArticleExist(id))
            {
                ModelState.AddModelError("Not Exist", "Post with this id is not exist");
            }

            Post article = await _context.Posts.FindAsync(id);
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return View(post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, Post post)
        {
            if (!IsArticleExist(id))
            {
                ModelState.AddModelError("Not Exist", "Post with this id is not exist");
            }
            
            Post article = await _context.Posts.FindAsync(id);
            if (!IsUserHaveAccessToPost(User.Identity, article))
            {
                ModelState.AddModelError("Not Exist", "You have no access to do this action");
            }

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return View(article);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            if (!IsArticleExist(id))
            {
                ModelState.AddModelError("Not Exist", "Post with this id is not exist");
            }
            
            Post article = await _context.Posts.FindAsync(id);
            if (!IsUserHaveAccessToPost(User.Identity, article))
            {
                ModelState.AddModelError("Not Exist", "You have no access to do this action");
            }
            _context.Posts.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetArticles", "Article");
        }

        private bool IsArticleExist(int id) => _context.Posts.Any(e => e.PostId == id);
        
        private bool IsUserHaveAccessToPost(IIdentity user, Post post) {
            User currentUser = _userManager.FindByNameAsync(user.Name).Result;
            return currentUser.IsAdmin || currentUser.OneToManyPosts.Contains(post);
        }
    }
}