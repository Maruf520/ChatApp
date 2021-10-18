using ChatApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> sIgninManager;
        private readonly UserManager<User> userManager;
        public AccountController(SignInManager<User> sIgninManager, UserManager<User> userManager)
        {
            this.sIgninManager = sIgninManager;
            this.userManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {

            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {


                var result = await sIgninManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            User user = new User
            {
                UserName = username
            };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await sIgninManager.SignInAsync(user, false);


                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Register", "Account");
        }

        public async Task<IActionResult> Logout()
        {
            await sIgninManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
