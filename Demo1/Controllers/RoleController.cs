using Demo1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo1.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {

            List<IdentityRole> roles = roleManager.Roles.ToList();
            List<RoleViewModel> result = new List<RoleViewModel>();
            foreach (var item in roles)
            {
                result.Add(new RoleViewModel() { Id = item.Id, Name = item.Name });
            }
            return View(result);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (!await roleManager.RoleExistsAsync(model.Name))
                {
                    await roleManager.CreateAsync(new IdentityRole(model.Name));
                }
            }
            return View();
        }
    }
}
