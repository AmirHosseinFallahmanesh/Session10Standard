using Demo1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public AccountController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }


        public IActionResult Index()
        {

            var users = userManager.Users.ToList();

            List<UserViewModel> userViewModels = new List<UserViewModel>();
            foreach (var item in users)
            {
                UserViewModel userViewModel = new UserViewModel()
                {
                    Id = item.Id,
                    Email = item.Email,
                    Password = item.PasswordHash,
                    Username = item.UserName
                };
                userViewModels.Add(userViewModel);

            }

            return View(userViewModels);
            
        }


        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = model.Username,
                    Email = model.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //email confirme
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }



        public async Task<IActionResult> Delete(string Id)
        {
            IdentityUser user =await userManager.FindByIdAsync(Id);
            if (user!=null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
            }
           


            return RedirectToAction("index");
        }





    }


}
