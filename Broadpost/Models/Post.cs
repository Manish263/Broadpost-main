using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Broadpost.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Display(Name = "Post Message")]
        public string PostMessage { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }


        [ForeignKey("Channel")]
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }


        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
