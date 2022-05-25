using IT_Forum.Helpers;
using IT_Forum.Models;
using IT_Forum.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IT_Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly Context _context;

        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, Context context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? page)
        {
            if (page is not null && page <= 1)
                page = 1;

            var storage = new List<PostViewModel>();
            foreach (var post in _context.Posts.OrderByDescending(post => post.PostId).ToList())
            {
                var user = _userManager.Users.FirstOrDefault(u => u.Id == post.UserId);
                post.Creator = user;
                
                var likes = _context.Likes.Where(l => l.PostId == post.PostId).ToList().Count;
                var comments = _context.Comments.Where(c => c.PostId == post.PostId).ToList().Count;

                storage.Add(new PostViewModel(post, likes, comments));
            }
            var pageSize = 4;

            return View(await PaginationList<PostViewModel>.CreateAsync(storage, page ?? 1, pageSize));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
