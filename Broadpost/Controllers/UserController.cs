using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadpost.Models;
using Newtonsoft.Json;
using Broadpost.Mail;

namespace Broadpost.Controllers
{
    public class UserController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        //User Login============================================================================
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            using(var db = new BroadpostDbContext())
            {
                var entity = db.Users.FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);

                if(entity == null)
                {
                    ViewBag.Message = "Invalid Username/Password";
                    return View(user);
                }

                HttpContext.Session.SetString("UserId", entity.UserId.ToString());
                HttpContext.Session.SetString("UserName", entity.Name);
                return RedirectToAction("Index","Dashboard");
            }
        }



        //User Logout============================================================================
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction(nameof(Login));
        }



        //User Register============================================================================
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                using(var db = new BroadpostDbContext())
                {
                    var isUserExist = db.Users.FirstOrDefault(u => u.Email == user.Email || u.UserName == user.UserName);
                    if(isUserExist == null)
                    {
                        //adding user
                        db.Users.Add(user);
                        db.SaveChanges();

                        //sending welcome mail
                        MailModel mail = new MailModel()
                        {
                            ReceiverName = user.Name,
                            ReceiverAddress = user.Email,
                            Subject = "Registration successfull",
                            Message = $"Thankyou {user.Name} for the registration, we welcome you in BroadPost"
                        };
                        MailHandler.SendMail(mail);

                        return RedirectToAction(nameof(Login));
                    }
                    else if(db.Users.FirstOrDefault(u => u.Email == user.Email) != null)
                    {
                        ViewBag.Message = "Email is already registered";
                        return View(user);
                    }

                    ViewBag.Message = "User Name is already taken";
                    return View(user);
                }
            }
            return View(user);
        }


        //Forgot Password============================================================================
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string Email)
        {
            using (var db = new BroadpostDbContext())
            {
                var entity = db.Users.FirstOrDefault(u => u.Email.ToLower() == Email.ToLower());
                if (entity != null)
                {
                    //Sending mail of forgot password
                    MailModel mail = new MailModel
                    {
                        ReceiverAddress = Email,
                        Subject = "Forgot Password",
                        Message = $"Your BroadPost password is \"{entity.Password}\""
                    };
                    MailHandler.SendMail(mail);
                    ViewBag.message = "We have Sent Your password over your mail go and check them out";
                    return View();
                }
                ViewBag.message = "Incorrect Email";
                return View();
            }
        }

        //Getting channel users============================================================================
        public string getChannelUsers([FromBody] ChannelIdBinderr obj)
        {
            using(var db = new BroadpostDbContext())
            {
                var channelId = Convert.ToInt32(obj.ChannelId);

                var entities =  from u in db.Users
                                where !(from cu in db.ChannelUsers
                                        where cu.ChannelId == Convert.ToInt32(channelId)
                                        select cu.UserId)
                                        .Contains(u.UserId) &&
                                        !(from i in db.Invitations
                                          where i.ChannelId == channelId
                                          select i.ReceverUserId)
                                          .Contains(u.UserId)
                              select new 
                              { 
                                u.UserId,
                                u.Name
                              };

                return JsonConvert.SerializeObject(entities);

            }
        }
    }

    public class ChannelIdBinderr
    {
        public string ChannelId { get; set; }
    }


}
