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
        private readonly IPasswordValidator<IdentityUser> passwordValidator;
        private readonly IPasswordHasher<IdentityUser> passwordHasher;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager
            ,IPasswordValidator<IdentityUser> passwordValidator,
            IPasswordHasher<IdentityUser> passwordHasher,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.passwordValidator = passwordValidator;
            this.passwordHasher = passwordHasher;
            this.signInManager = signInManager;
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

        public IActionResult Login(string ReturnUrl)
        {
            LoginViewModel model = new LoginViewModel();
            model.ReturnUrl = ReturnUrl;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByEmailAsync(model.Email);
                if (user==null)
                {
                    ModelState.AddModelError("", "Username Not Found");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    return LocalRedirect(model.ReturnUrl??"/");
                }

            }

            return View(model);


        }

        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
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
            IdentityUser user = await userManager.FindByIdAsync(Id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
            }



            return RedirectToAction("index");
        }

        public async Task<IActionResult> Edit(string Id)
        {
            IdentityUser user = await userManager.FindByIdAsync(Id);
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email,

                Username = user.UserName
            };

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            IdentityUser user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {

                ModelState.AddModelError("", "User Not Found");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                user.UserName = model.Username;
                user.Email = model.Email;
            }
            //var modelValidatorResult = await userValidator.ValidateAsync(userManager, user);

            //if (!modelValidatorResult.Succeeded)
            //{
            //    AddModelError(modelValidatorResult);

            //}
            var passwordValidateResult = await passwordValidator.ValidateAsync(userManager, user, model.Password);
            if (!passwordValidateResult.Succeeded)
            {
                foreach (var item in passwordValidateResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

            }
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
            IdentityResult result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View(model);
        }


    }


}
