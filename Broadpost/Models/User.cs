using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Broadpost.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(UserName), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Region { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage = "User name can't be empty")]
        [StringLength(20,MinimumLength = 5,ErrorMessage = "UserName has to be between 5 to 20 characters")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password should be of 8-20 characters long")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Password Verify")]
        [Compare("Password", ErrorMessage = "Verify password is incorrect")]
        public string PasswordVerify { get; set; }


        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public List<Post> Posts { get; set; }

        public List<Channel> Channels { get; set; }

        public List<Invitation> Invitations { get; set; }

        public List<ChannelUser> ChannelUsers { get; set; }



    }
}
