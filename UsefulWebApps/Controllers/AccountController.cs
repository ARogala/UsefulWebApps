﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsefulWebApps.IdentityModels;

namespace UsefulWebApps.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        //register a StandardUser
        [HttpPost]
        public async Task<IActionResult> Register(Register userRegInfo)
        {
            if (!ModelState.IsValid) { return View(); }

            IdentityUser user = new IdentityUser
            {
                Email = userRegInfo.Email,
                UserName = userRegInfo.UserName
            };
            IdentityResult result = await _userManager.CreateAsync(user, userRegInfo.Password);
            if (result.Succeeded)
            {

                if (!await _roleManager.RoleExistsAsync("StandardUser"))
                {
                    IdentityRole standerUserRole = new IdentityRole("StandardUser");
                    await _roleManager.CreateAsync(standerUserRole);
                }
                await _userManager.AddToRoleAsync(user, "StandardUser");
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Credential userLoginInfo)
        {
            if (!ModelState.IsValid) { return View(); }

            var result = await _signInManager.PasswordSignInAsync(
                    userLoginInfo.UserName,
                    userLoginInfo.Password,
                    userLoginInfo.RememberMe,
                    false
                );
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to login");
                }
                return View();
            }
        }
    }
}