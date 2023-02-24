using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Broadpost.Models
{
    public class ChannelUser
    {
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }


        public int UserId { get; set; }
        public User User { get; set; }

    }
}
