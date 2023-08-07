using Microsoft.AspNetCore.Identity;

namespace DemoRazorWebApp.Areas.Identity.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
    }
}
