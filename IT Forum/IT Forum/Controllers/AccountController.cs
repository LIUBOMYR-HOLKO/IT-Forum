using IT_Forum.Models;
using IT_Forum.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IT_Forum.Controllers
{
    public class AccountController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(Context context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> RegisterAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
               
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> LoginAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    bool isPasswordMatches = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (isPasswordMatches)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Password doesn\'t matches");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SignOutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
