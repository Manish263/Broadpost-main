using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Broadpost.Models
{
    [Index(nameof(ChannelName), IsUnique = true)]
    public class Channel
    {
        [Key]
        public int ChannelId { get; set; }

        [Required]
        [Display(Name = "Channel Name")]
        public string ChannelName { get; set; }

        [Display(Name = "Channel Description")]
        [Column(TypeName = "varchar(200)")]
        public string ChannelDesc { get; set; }

        public string Tags { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public string Admin {get;set;}

        public int TotalUser { get; set; } = 1;
        public int TotalPost { get; set; } = 0;


        public List<Post> Posts { get; set; }

        public List<Invitation> Invitations { get; set; }

        public List<ChannelUser> ChannelUsers { get; set; }

    }
}
