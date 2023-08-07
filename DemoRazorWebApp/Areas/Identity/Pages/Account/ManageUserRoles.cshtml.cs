using DemoRazorWebApp.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static DemoRazorWebApp.Areas.Identity.Pages.Account.RolesModel;

namespace DemoRazorWebApp.Areas.Identity.Pages.Account
{
    public class ManageUserRolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUserRolesModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }

        [BindProperty]
        public List<ManageUserRolesViewModel> UserRolesManager { get; set; }

        public class ManageUserRolesViewModel
        {
            public string RoleId { get; set; }
            public string RoleName { get; set; }
            public bool Selected { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            UserId = userId;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                //ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return NotFound();
            }
            //ViewBag.UserName = user.UserName;
            UserRolesManager = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                UserRolesManager.Add(userRolesViewModel);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                try
                {
                    var user = await _userManager.FindByIdAsync(UserId);
                    if (user == null)
                    {
                        return NotFound("User Not Found");
                    }
                    var roles = await _userManager.GetRolesAsync(user);
                    var result = await _userManager.RemoveFromRolesAsync(user, roles);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot remove user existing roles");
                        return Page();
                    }
                    result = await _userManager.AddToRolesAsync(user, UserRolesManager.Where(x => x.Selected).Select(y => y.RoleName));
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Cannot add selected roles to user");
                        return Page();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return RedirectToPage("/Account/Roles", new { Area = "Identity" });
        }
    }
}
