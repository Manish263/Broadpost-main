using MailKit.Net.Smtp;
using MimeKit;

namespace Broadpost.Mail
{
    public class MailHandler
    {
        public static void SendMail(MailModel mailModel)
        {
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("Broad Post", "ProjectWork338@gmail.com"));
            mail.To.Add(new MailboxAddress(mailModel.ReceiverName, mailModel.ReceiverAddress));
            mail.Subject = mailModel.Subject;
            mail.Body = new TextPart("plain")
            {
                Text = mailModel.Message
            };
            using(var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false); 
                client.Authenticate("ProjectWork338@gmail.com", "visualstudio");

                client.Send(mail);

                client.Disconnect(true);
            }
        }
    }
}
