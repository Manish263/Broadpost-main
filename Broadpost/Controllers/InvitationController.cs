using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadpost.Models;
using Broadpost.Mail;

namespace Broadpost.Controllers
{
    public class InvitationController : Controller
    {
        private int _sessionUserId;
        private string _sessionUserName;
        private bool isSessionValid()
        {
            _sessionUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            _sessionUserName = HttpContext.Session.GetString("UserName");
            byte[] data;
            return HttpContext.Session.TryGetValue("UserId", out data);
        }


        public IActionResult Index()
        {
            if (isSessionValid())
            {
                var db = new BroadpostDbContext();

                var entity = from i in db.Invitations
                             where i.ReceverUserId == _sessionUserId
                             select i;

                var entitys = (from c in db.Channels
                             join us in entity
                             on c.ChannelId equals us.ChannelId
                             select c).ToList();

                ViewBag.invitations = entitys;

                return View();

            }
            return RedirectToAction("Login", "User");
        }

        


        [HttpPost]
        public void Invite([FromBody] Invitation invitation)
        {
            if (isSessionValid())
            {
                using(var db = new BroadpostDbContext())
                {
                    var entity = db.Invitations.FirstOrDefault(i => i.ChannelId == invitation.ChannelId
                                                        && i.ReceverUserId == invitation.ReceverUserId);
                    if(entity == null)
                    {
                        //Adding invite in Invitation
                        db.Invitations.Add(invitation);
                        db.SaveChanges();

                        //Sending Invitation Mail
                        var reciverUserEntity = (from u in db.Users where u.UserId == invitation.ReceverUserId
                                                select new { u.Name, u.Email}).FirstOrDefault();
                        var channelName = db.Channels.FirstOrDefault(c => c.ChannelId == invitation.ChannelId).ChannelName;
                        MailModel mail = new MailModel()
                        {
                            ReceiverName = reciverUserEntity.Name,
                            ReceiverAddress = reciverUserEntity.Email,
                            Subject = "New Channel Join Request",
                            Message = $"{_sessionUserName} have sent you request to join his channel \"{channelName}\""
                        };
                        MailHandler.SendMail(mail);
                    }
                }
            }
        }


        [HttpPost]
        public void HandleInviteResponse([FromBody] Invitation invitation)
        {
            if (isSessionValid())
            {
                using (var db = new BroadpostDbContext())
                {
                    var entity = db.Invitations.FirstOrDefault(i => i.ChannelId == invitation.ChannelId && i.ReceverUserId == _sessionUserId);
                    if(invitation.Status == 1) //Accept Invitatiom
                    {
                        //updating Total Users in channel
                        var channelEntity = db.Channels.FirstOrDefault(c => c.ChannelId == invitation.ChannelId);
                        channelEntity.TotalUser++;

                        //Adding new channel user to ChannelUser
                        var channelUser = new ChannelUser();
                        channelUser.ChannelId = entity.ChannelId;
                        channelUser.UserId = entity.ReceverUserId;

                        db.ChannelUsers.Add(channelUser);

                        //Removing from Invitation
                        db.Invitations.Remove(entity);
                        db.SaveChanges();
                    }
                    else if(invitation.Status == 2) //Decline Invitation
                    {
                        //Removing from Invitation
                        db.Invitations.Remove(entity);
                        db.SaveChanges();
                    }
                }
            }
        }


    }


}
