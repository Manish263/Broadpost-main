using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadpost.Models;
using System.Collections;

namespace Broadpost.Controllers
{
    public class DashboardController : Controller
    {
        private int _sessionUserId;
        private bool isSessionValid()
        {
            _sessionUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            byte[] data;
            return HttpContext.Session.TryGetValue("UserId", out data);
        }

        //Dashboard Index============================================================================
        public ActionResult Index()
        {
            if( isSessionValid() )
            {
                ViewBag.trendings = new ArrayList()
                {
                    GetTrendingChannels(),
                    GetTrendingTags(),
                    GetTrendingRegions(),
                    GetTrendingUsers()
                };
                return View();
            }
            return RedirectToAction("Login", "User");
        }


            









//=====================================================================================================================================
        //User Profile============================================================================
        public ActionResult Profile()
        {
            if (isSessionValid())
            {
                using(var db = new BroadpostDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.UserId == _sessionUserId);
                    return View(user);
                }
            }
            return RedirectToAction("Login","User");
        }


        //Edit User Profile============================================================================
        public ActionResult EditProfile(int? id)
        {
            if (isSessionValid())
            {
                if(id == _sessionUserId)
                {
                    using (var db = new BroadpostDbContext())
                    {
                        var entity = db.Users.FirstOrDefault(u=>u.UserId == id);
                        if (entity == null)
                        {
                            return NotFound();
                        }
                        return View(entity);
                    }
                }
                return RedirectToAction(nameof(Profile));
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult EditProfile(User user)
        {
            if (isSessionValid())
            {
                if (user.UserId == _sessionUserId)
                {
                    if (ModelState.IsValid)
                    {
                        using (var db = new BroadpostDbContext())
                        {
                            var entity = db.Users.FirstOrDefault(c => c.UserId == user.UserId);
                            entity.UserName = user.UserName;
                            entity.Email = user.Email;
                            entity.Region = user.Region;
                            entity.Gender = user.Gender;
                            entity.Age = user.Age;
                            entity.UpdatedAt = DateTime.Now;

                            db.SaveChanges();

                            return RedirectToAction(nameof(Profile));
                        }
                    }
                    return View(user);
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Login", "User");
        }


        //Change Password============================================================================
        public ActionResult ChangePassword(int? id)
        {
            if (isSessionValid())
            {
                if(id == _sessionUserId)
                {
                    ChangePassword changePassword = new ChangePassword
                    {
                        UserId = (int)id
                    };
                    return View(changePassword);
                }
                return RedirectToAction("Profile");
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public ActionResult ChangePassword(ChangePassword userPassword)
        {
            if (isSessionValid())
            {
                if(userPassword.UserId == _sessionUserId)
                {
                    using (var db = new BroadpostDbContext())
                    {
                        var entity = db.Users.FirstOrDefault(u => u.UserId == _sessionUserId);
                        if(entity.Password == userPassword.CurrentPassword)
                        {
                            entity.Password = userPassword.NewPassword;
                            entity.PasswordVerify = userPassword.PasswordVerify;

                            db.SaveChanges();

                            ViewBag.message = "Password changed successfully";
                            return View();
                        }
                        ViewBag.message = "Current Password is Wrong";
                        return View();
                    }
                }
                return RedirectToAction("Profile");
            }
            return RedirectToAction("Login", "User");
        }




        //Required Functions============================================================================
        //Top 5 Trending Channels having most number of posts
        private IEnumerable GetTrendingChannels()
        {
            using(var db = new BroadpostDbContext())
            {
                var entities = db.Channels.OrderByDescending(c => c.TotalPost).Take(5).ToList();
                return entities;
            }
        }

        //Top 5 Trending Tags
        private IEnumerable GetTrendingTags()
        {
            using (var db = new BroadpostDbContext())
            {
                var TagsList = from c in db.Channels select new { c.Tags };
                var AllTagsDict = new Dictionary<string, int>();
                
                char[] separator = { ',', ' ' };
                foreach(var tagsString in TagsList)
                {
                    string[] tags = tagsString.Tags.ToString().Split(separator,StringSplitOptions.RemoveEmptyEntries);
                  
                    foreach(var tag in tags)
                    {
                        if (AllTagsDict.ContainsKey(tag.ToLower()))
                            AllTagsDict[tag.ToLower()]++;
                        else
                            AllTagsDict.Add(tag.ToLower(), 1);
                    }
                }
                var trendingTags = AllTagsDict.OrderByDescending(v => v.Value).Take(5);

                return trendingTags;
            }
        }

        //Top 5 Trending Regions having most number of users
        private IEnumerable GetTrendingRegions()
        {
            using(var db = new BroadpostDbContext())
            {
                var topRegions = (from u in db.Users
                                group u by u.Region.ToLower()
                                into g
                                select new
                                {
                                    Region = g.Key,
                                    Count = g.Count()
                                }).OrderByDescending(tu => tu.Count).Take(5).ToList();
                Dictionary<string, int> d= new Dictionary<string, int>();
                foreach(var i in topRegions)
                {
                    d.Add(i.Region, i.Count);
                }
                return d;   
            }
        }


        //Top 5 Users having most no. of posts
        private IEnumerable GetTrendingUsers()
        {
            using(var db = new BroadpostDbContext())
            {
                var topUsers = (from p in db.Posts
                                group p by p.UserId
                                into u
                                select new
                                {
                                    UserId = u.Key,
                                    Count = u.Count()
                                }).OrderByDescending(tu => tu.Count).Take(5);
                var r = from tu in topUsers
                        join u in db.Users
                        on tu.UserId equals u.UserId
                        select new
                        {
                            u.UserName,
                            tu.Count
                        };
                Dictionary<string, int> d = new Dictionary<string, int>();
                foreach (var i in r)
                {
                    d.Add(i.UserName, i.Count);
                }
                return d;
            }
        }



    }

}
