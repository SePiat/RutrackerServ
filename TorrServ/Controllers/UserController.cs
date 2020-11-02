using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TorrServ.ViewModels;

namespace TorrServ.Controllers
{
    public class UserController : Controller
    {
       
            private UserManager<IdentityUser> _userManager;

            public UserController(UserManager<IdentityUser> userManager)
            {
                _userManager = userManager;
            }

        // GET: /<controller>/
        [Authorize(Roles = "admin")]
        public IActionResult Index() => View(_userManager.Users.ToList());
       [Authorize(Roles = "admin")]
        public IActionResult Create() => View();
        [Authorize(Roles = "admin")]
        [HttpPost]
            public async Task<IActionResult> Create(CreateUserViewModel model)
            {
                if (ModelState.IsValid)
                {
                IdentityUser user = new IdentityUser { Email = model.Email, UserName = model.Email};
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(String.Empty, error.Description);
                        }
                    }

                }
                return View(model);
            }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string Id)
            {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
                if (user == null)
                {
                    return NotFound();
                }

                EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email };
                return View(model);
            }
        [Authorize(Roles = "admin")]
        [HttpPost]
            public async Task<IActionResult> Edit(EditUserViewModel model)
            {
                if (ModelState.IsValid)
                {
                IdentityUser user = await _userManager.FindByIdAsync(model.Id);
                    if (user != null)
                    {
                        user.Email = model.Email;
                        user.UserName = model.Email;
                        

                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                }

                return View(model);

            }
        [Authorize(Roles = "admin")]
        [HttpPost]
            public async Task<IActionResult> Delete(string Id)
            {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
                if (user != null)
                {
                    IdentityResult result = await _userManager.DeleteAsync(user);
                }

                return RedirectToAction("Index");
            }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangePassword(string id)
            {
            IdentityUser user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
                return View(model);
            }
        [Authorize(Roles = "admin")]
        [HttpPost]
            public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
            {
                if (ModelState.IsValid)
                {
                IdentityUser user = await _userManager.FindByIdAsync(model.Id);
                    if (user != null)
                    {
                        var _passwordValidator =
                            HttpContext.RequestServices.GetService(typeof(IPasswordValidator<IdentityUser>)) as
                                IPasswordValidator<IdentityUser>;
                        var _passwordHasher =
                            HttpContext.RequestServices.GetService(typeof(IPasswordHasher<IdentityUser>)) as
                                IPasswordHasher<IdentityUser>;

                        IdentityResult result =
                            await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                        if (result.Succeeded)
                        {
                            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                            await _userManager.UpdateAsync(user);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Пользователь ненайден");
                    }
                }

                return View(model);

                
            }
        
    }
}