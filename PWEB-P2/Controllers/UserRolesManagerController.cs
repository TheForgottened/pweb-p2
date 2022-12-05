using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWEB_P2.Models;
using PWEB_P2.ViewModels;

namespace PWEB_P2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRolesManagerController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                userRolesViewModel.Add(new UserRolesViewModel()
                    {
                        UserId = user.Id,
                        PrimeiroNome = user.PrimeiroNome,
                        UltimoNome = user.UltimoNome,
                        UserName = user.UserName,
                        Roles = await _userManager.GetRolesAsync(user)
                    }
                );
            }

            return View(userRolesViewModel);
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        public async Task<IActionResult> Details(string userId)
        {
            var model = new List<ManageUserRolesViewModel>();
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _roleManager.Roles.ToListAsync();

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            ViewBag.UserName = user.UserName;

            foreach (var role in roles)
            {
                model.Add(new ManageUserRolesViewModel()
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        Selected = await _userManager.IsInRoleAsync(user, role.Name)
                    }
                );
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Details(List<ManageUserRolesViewModel> model,
            string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Unable to remove existing roles");
                return View();
            }

            result = await _userManager.AddToRolesAsync(
                user, 
                model
                    .Where(r => r.Selected)
                    .Select(r => r.RoleName)
                );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Unable to add new roles");
                return View();
            }

            return RedirectToAction("Index");
        }
    }
}
