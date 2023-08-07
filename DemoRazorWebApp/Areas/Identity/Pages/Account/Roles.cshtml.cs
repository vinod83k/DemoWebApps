using DemoRazorWebApp.Areas.Identity.Models;
using DemoRazorWebApp.Areas.Identity.Pages.Account.Manage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DemoRazorWebApp.Areas.Identity.Pages.Account
{
    public class RolesModel : PageModel
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public RolesModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<PersonalDataModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]        
        public List<UserRolesModel> UserRoles { get; set; }

        public class UserRolesModel
        {
            public string UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public IEnumerable<string> Roles { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //var user = await _userManager.GetUserAsync(User);
            //var roles = _roleManager.Roles.Select(x => x.Name).ToList();

            //Roles = roles;

            UserRoles = new List<UserRolesModel>();

            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesModel>();
            foreach (ApplicationUser user in users)
            {
                var thisViewModel = new UserRolesModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(thisViewModel);
            }

            UserRoles = userRolesViewModel;

            return Page();
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
}
