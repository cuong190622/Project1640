﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Project1640.EF;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Project1640.Controllers
{
    public class StaffController : Controller
    {

        public ActionResult Index(int id = 1)
        {
            using (var dbCT = new EF.CMSContext())
            {
                int Count = dbCT.Idea.Count();
                if (Count <= 5)
                {
                    TempData["PageNo"] = 1;
                    TempData["PageMax"] = 1;
                    var ideas = dbCT.Idea.OrderBy(c => c.Id).ToList();
                    return View(ideas);
                }
                else
                {
                    var ideas = dbCT.Idea.OrderBy(c => c.Id).ToList();
                    if (Count % 5 != 0)
                    { 
                        TempData["PageMax"] = (Count / 5) + 1;
                    }
                    else
                    {
                        TempData["PageMax"] = (Count / 5);
                    }
                    TempData["PageNo"] = id;
                    return View(ideas);
                }

            }
        }

        [HttpGet]
        public ActionResult TestAddUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> TestAddUser(UserInfo a)
        {
            if (!ModelState.IsValid)//if user input wrong
            {
                return View(a);
            }
            else
            {
                var context = new CMSContext();
                var store = new UserStore<UserInfo>(context);
                var manager = new UserManager<UserInfo>(store);

                var user = await manager.FindByEmailAsync(a.Email);

                if (user == null)
                {
                    user = new UserInfo
                    {
                        UserName = a.Email.Split('@')[0],
                        Email = a.Email,
                        Age = a.Age,
                        Name = a.Name,
                        Role = "trainer",
                        PasswordHash = "123qwe123",
                        DepartmentId = 1

                    };
                    await manager.CreateAsync(user, user.PasswordHash);
                }
            }
            return RedirectToAction("Index");
        }

        private List<SelectListItem> getList()
        {
            using (var abc = new EF.CMSContext()) //create a new value abc is an object of CMSContext
            {
                var stx = abc.Category.Select(p => new SelectListItem //Select anonymous
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();

                return stx;
            }
        }


        [HttpGet]
        public ActionResult CreateIdea()
        {
            ViewBag.Class = getList();
            return View();
        }

        private List<Category> Convert(EF.CMSContext database, string formatIds)
        {
            if (formatIds != null)
            {
                var abc = formatIds.Split(',').Select(id => Int32.Parse(id)).ToArray();
                return database.Category.Where(f => abc.Contains(f.Id)).ToList();
            }
            else
            {
                return database.Category.Where(c => c.Id == 0).ToList();
            }
        }

        private void SetViewBag()
        {
            using (var bwCtx = new EF.CMSContext())// use a variable named bwCtx of CMSContext 
            {            
                ViewBag.Formats = bwCtx.Category.ToList(); //select all data from Courses in DbSet
            }
        }


        [HttpPost]
        public ActionResult CreateIdea(Idea a, FormCollection f, HttpPostedFileBase postedFile)
        {
            if (!ModelState.IsValid)//if user input wrong
            {
                TempData["abc"] = f["formatIds[]"];
                SetViewBag();// call function get viewbag to return the data when the user input wrong
                return View(a);
            }
            else
            {

                var context = new CMSContext();
                using (var Database = new EF.CMSContext())
                {
                    a.Status = !a.Status;
                    a.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    
                    a.UserId = "8c495b46-2b8b-4e41-a183-aac1b9e250bf";
                    Database.Idea.Add(a);
                    Database.SaveChanges();
                    SaveFile(new FileUpload(), postedFile, a.Id);

                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public void SaveFile(FileUpload a, HttpPostedFileBase postedFile, int IdeaId)
        {
            if (postedFile != null)
            {
                string _FileName = Path.GetFileName(postedFile.FileName);
                string _path = Path.Combine(Server.MapPath("~/FileUpload"), _FileName);
                postedFile.SaveAs(_path);

                using (var Database = new EF.CMSContext())
                {
                    a.Name = postedFile.FileName;
                    a.Url = _path;
                    a.UserId = "8c495b46-2b8b-4e41-a183-aac1b9e250bf";
                    a.IdeaId = IdeaId;
                    Database.File.Add(a);
                    Database.SaveChanges();
                }
            }

        }

        public ActionResult ViewIdea(int id)
        {
            using (var FAPCtx = new EF.CMSContext())
            {
                var _idea = FAPCtx.Idea.FirstOrDefault(c => c.Id == id);

                if (_idea != null)
                {
                    _idea.Views++;
                    FAPCtx.SaveChanges();
                    TempData["IdeaId"] = id;
                    return View(_idea);
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
        }

        public ActionResult ShowComment(int id)
        {

                using (var dbCT = new EF.CMSContext())
                {
                    var _comment = dbCT.Comment
                                            .Where(c => c.IdeaId == id)
                                            .ToList();
                    if (_comment.Count != 0)
                    {
                        return View(_comment);
                    }
                    else
                    {
                        return Content($"No Comment yet!");
                    }
                   
                }
        }
        [HttpGet]
        public ActionResult CreateComment()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateComment(Comment a, int id)
        {
            using (var database = new EF.CMSContext())
            {
                a.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                a.UserId = "8c495b46-2b8b-4e41-a183-aac1b9e250bf";
                a.IdeaId = id;
                a.Status = !a.Status;
                database.Comment.Add(a);
                database.SaveChanges();
                await SendEmail("123456789awdstk.mk@gmail.com", a.Content);
                TempData["IdeaId"] = id;
            }
            return RedirectToAction("ViewIdea", new { id = id });
        }
        public ActionResult ShowCategory(int id)
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _category = dbCT.Category
                                        .Where(c => c.Id == id)
                                        .ToList();
             return View(_category);
            }
        }

        [HttpGet]
        public ActionResult Like()
        {
            Getlike(Int32.Parse(TempData["IdeaId"].ToString()), "8c495b46-2b8b-4e41-a183-aac1b9e250bf");
            return View();
        }

        [HttpPost]
        public ActionResult Like(React a, string like)
        {            
            using (var database = new EF.CMSContext())
            {
                int react = database.React.Where(c => c.IdeaId == a.IdeaId).Count();
                if(react != 0)
                {
                    var listLiked = database.React.Where(c => c.IdeaId == a.IdeaId).Where(c => c.UserId == "8c495b46-2b8b-4e41-a183-aac1b9e250bf");
                    database.React.RemoveRange(listLiked);
                }
                if(like == "Up")
                {
                    a.React_Type = true;
                }else a.React_Type = false;
                a.UserId = "8c495b46-2b8b-4e41-a183-aac1b9e250bf";
                database.React.Add(a);
                database.SaveChanges();
                TempData["IdeaId"] = a.IdeaId;
            }
            return RedirectToAction("ViewIdea", new { id = a.IdeaId });
        }
        
        public void Getlike(int IdeaID, string UserId)
        {
            using (var dbCT = new EF.CMSContext())
            {
                int like = dbCT.React.Where(c => c.IdeaId == IdeaID).Where(c => c.React_Type == true).Count();
                int dislike = dbCT.React.Where(c => c.IdeaId == IdeaID).Where(c => c.React_Type == false).Count();
                TempData["LikeCount"] = like - dislike;
                int Userlike = dbCT.React.Where(c => c.IdeaId == IdeaID).Where(c => c.React_Type == true).Where(c => c.UserId == UserId).Count();
                int Userdislike = dbCT.React.Where(c => c.IdeaId == IdeaID).Where(c => c.React_Type == false).Where(c => c.UserId == UserId).Count();
                if(Userdislike != 0)
                {
                    TempData["LikeStatus"] = "Dislike";
                }if(Userlike != 0)
                {
                    TempData["LikeStatus"] = "Like";
                }
                using (var database = new EF.CMSContext())
                {
                    var _idea = database.Idea.FirstOrDefault(c => c.Id == IdeaID);
                    _idea.Rank = like - dislike;
                }
            }
        }

        public ActionResult TopView()
        {
            using (var dbCT = new EF.CMSContext())
            {
                var _idea = dbCT.Idea.OrderByDescending(c => c.Views).First();
                return RedirectToAction("ViewIdea", new { id = _idea.Id });
            }

        }
        public ActionResult TopLike()
        {
            using (var dbCT = new EF.CMSContext())
            {
                var _idea = dbCT.Idea.OrderByDescending(c => c.Rank).First();
                return RedirectToAction("ViewIdea", new { id = _idea.Id });
            }

        }

        public ActionResult LastIdea()
        {
            using (var dbCT = new EF.CMSContext())
            {
                var _idea = dbCT.Idea.OrderByDescending(c => c.Id).First();
                return RedirectToAction("ViewIdea", new { id = _idea.Id });
            }

        }

        public ActionResult LastComment()
        {
            using (var dbCT = new EF.CMSContext())
            {
                var _comment = dbCT.Comment.OrderByDescending(c => c.Id).First();
                TempData["LastComment"] = _comment.Id;
                return RedirectToAction("ViewIdea", new { id = _comment.IdeaId });
                
            }

        }
        public async Task SendEmail(string email, string comment)
        {
            var body = "<p>Email From: {0} </p><p>Message: {1}</p> <p>Comment: {2}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));  
            message.From = new MailAddress("ASPxyz123ab@gmail.com");  
            message.Subject = "Someone comment in your post";
            message.Body = string.Format(body, "Admin", "New user comment on your post!", comment);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "ASPxyz123ab@gmail.com",  
                    Password = "123456789awds"  
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
        }
    }
}