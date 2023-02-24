using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Broadpost.Models
{
    public class ChangePassword
    {
        public int UserId { get; set; }

        [Required,Display(Name ="Current Password")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password should be of 8-20 characters long")]
        public string CurrentPassword { get; set; }

        [Required,Display(Name ="New Password")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password should be of 8-20 characters long")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Password Verify")]
        [Compare("NewPassword", ErrorMessage = "Verify password is incorrect")]
        public string PasswordVerify { get; set; }
    }
}
