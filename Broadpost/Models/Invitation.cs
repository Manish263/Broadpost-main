using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Broadpost.Models
{
   
    public class Invitation
    {
        [ForeignKey("Channel")]
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }

        [ForeignKey("User")]
        public int ReceverUserId { get; set; }
        public User User { get; set; }

        public int Status { get; set; } = 0;
    }
}
