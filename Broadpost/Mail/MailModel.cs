using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Broadpost.Mail
{
    public class MailModel
    {
        [Required]
        public string ReceiverAddress { get; set; }
        public string ReceiverName { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
