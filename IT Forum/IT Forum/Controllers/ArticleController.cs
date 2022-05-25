using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using IT_Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IT_Forum.Helpers;
using IT_Forum.Models.Entities;
using IT_Forum.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("GetArticle/{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            if (!IsArticleExist(id))
            {
                ModelState.AddModelError("Not Exist", "Post with this id is not exist");
            }

            Post article = await _context.Posts.FindAsync(id);
            User user = CurrentUser(User.Identity);
            bool isLiked = user != null && _context.Likes.Any(like => like.PostId == id && like.UserId == user.Id);
            bool isHaveAccessToUpdate = user != null && IsUserHaveAccessToPost(User.Identity, article);
            List<Comment> comments = _context.Comments.Where(comment => comment.PostId == id && comment.Context != "")
                .OrderByDescending(comment => comment.CommentId).ToList();
            PostViewModel model = new PostViewModel(article, isLiked, isHaveAccessToUpdate, comments);
            return View(model);
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Post post)
        {
            post.Creator = CurrentUser(User.Identity);
            post.UserId = post.Creator.Id;
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetArticle", new {id = post.PostId});
        }
        
        [HttpGet("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id)
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
            
            return View(article);
        }
        
        [HttpPost("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, Post post)
        {
            Post article = await _context.Posts.FindAsync(id);
            article.Context = post.Context;
            article.Title = post.Title;
            if (!IsUserHaveAccessToPost(User.Identity, article))
            {
                ModelState.AddModelError("Not Exist", "You have no access to do this action");
            }
            
            _context.Entry(article).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsArticleExist(id))
                {
                    ModelState.AddModelError("Not Exist", "Post with this id is not exist");
                    return NoContent();
                }
                throw;
            }
            
            return RedirectToAction("GetArticle", new {id = id});
        }

        [HttpPost("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
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

        private bool IsArticleExist(int id) => _context.Posts.FirstOrDefault(e => e.PostId == id) is not null;
        
        private bool IsUserHaveAccessToPost(IIdentity user, Post post) {
            User currentUser = CurrentUser(user);
            if (currentUser.IsAdmin)
                return true;

            return post.Creator == currentUser;
        }

        private User CurrentUser(IIdentity user)
        {
            return user.Name is null ? null : _userManager.FindByNameAsync(user.Name).Result;
        }
    }
}