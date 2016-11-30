using BlogSystem.Models.DataModels;
using BlogSystem.Models.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BlogSystem.Models.ViewModels
{
    public class EditUserViewModel
    {
        public ApplicationUser User { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }

        public IList<Role> Roles { get; set; }
    }
}
