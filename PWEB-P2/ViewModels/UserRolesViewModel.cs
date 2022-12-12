using System.ComponentModel.DataAnnotations;

namespace PWEB_P2.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        public string PrimeiroNome { get; set; }

        [Display(Name = "Last Name")]
        public string UltimoNome { get; set; }

        [Display(Name = "Email")]
        public string UserName { get; set; }

        public byte[]? Avatar { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
