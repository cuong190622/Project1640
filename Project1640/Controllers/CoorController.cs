using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Project1640.EF;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace Project1640.Controllers
{
    public class CoorController : Controller
    {
        // GET: Coor
        public ActionResult Index(int id = 1, int categoryId = 0, string count = "a")
        {
            int number = 5;
            if (Regex.IsMatch(count, @"^\d+$"))
            {
                number = Int32.Parse(count);
            }
            if (categoryId == 0)
            {
                using (var dbCT = new EF.CMSContext())
                {
                    TempData["CategoryId"] = 0;
                    int Count = dbCT.Idea.Count();
                    if (Count <= number)
                    {
                        TempData["PageNo"] = 1;
                        TempData["PageMax"] = 1;
                        TempData["Number"] = number;
                        var ideas = dbCT.Idea.OrderBy(c => c.Id).ToList();
                        return View(ideas);
                    }
                    else
                    {
                        var ideas = dbCT.Idea.OrderBy(c => c.Id).ToList();
                        if (Count % number != 0)
                        {
                            TempData["PageMax"] = (Count / number) + 1;
                        }
                        else
                        {
                            TempData["PageMax"] = (Count / number);
                        }
                        TempData["Number"] = number;
                        TempData["PageNo"] = id;
                        return View(ideas);
                    }

                }
            }
            else
            {
                using (var dbCT = new EF.CMSContext())
                {
                    TempData["CategoryId"] = categoryId;
                    int Count = dbCT.Idea.Where(p => p.CategoryId == categoryId).Count();
                    if (Count <= number)
                    {
                        TempData["PageNo"] = 1;
                        TempData["PageMax"] = 1;
                        TempData["Number"] = number;
                        var ideas = dbCT.Idea.Where(p => p.CategoryId == categoryId).OrderBy(c => c.Id).ToList();
                        return View(ideas);
                    }
                    else
                    {
                        var ideas = dbCT.Idea.Where(p => p.CategoryId == categoryId).OrderBy(c => c.Id).ToList();
                        if (Count % number != 0)
                        {
                            TempData["PageMax"] = (Count / number) + 1;
                        }
                        else
                        {
                            TempData["PageMax"] = (Count / number);
                        }
                        TempData["Number"] = number;
                        TempData["PageNo"] = id;
                        return View(ideas);
                    }

                }
            }

        }


        public ActionResult TopView()
        {
            using (var dbCT = new EF.CMSContext())
            {
                var _idea = dbCT.Idea.OrderByDescending(c => c.Views).First();
                return RedirectToAction("ViewIdea", new { IdeaId = _idea.Id });
            }

        }
        public ActionResult TopLike()
        {
            using (var dbCT = new EF.CMSContext())
            {
                var _idea = dbCT.Idea.OrderByDescending(c => c.Rank).First();
                return RedirectToAction("ViewIdea", new { IdeaId = _idea.Id });
            }

        }

        public ActionResult LastIdea()
        {
            using (var dbCT = new EF.CMSContext())
            {
                var _idea = dbCT.Idea.OrderByDescending(c => c.Id).First();
                return RedirectToAction("ViewIdea", new { IdeaId = _idea.Id });
            }
        }

        public ActionResult LastComment()
        {
            using (var dbCT = new EF.CMSContext())
            {
                var _comment = dbCT.Comment.OrderByDescending(c => c.Id).First();
                TempData["LastComment"] = _comment.Id;
                return RedirectToAction("ViewIdea", new { IdeaId = _comment.IdeaId });
            }
        }
        public ActionResult ShowUser(string UserId)
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _category = dbCT.Users
                                        .Where(c => c.Id == UserId)
                                        .ToList();
                return View(_category);
            }
        }

        public async Task SendEmail(string email, string comment)
        {
            var body = "<p>Email From: {0} </p><p>Message: {1}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress("ASPxyz123ab@gmail.com");
            message.Subject = "New email form CHAT.com.vn";
            message.Body = string.Format(body, "Admin", comment);
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
        public async Task<string> FindIdUserByEmail(string email)
        {
            var context = new CMSContext();
            var store = new UserStore<UserInfo>(context);
            var manager = new UserManager<UserInfo>(store);
            var user = await manager.FindByEmailAsync(email);
            return user.Id.ToString();

        }

        public ActionResult ViewIdea(int IdeaId)
        {
            using (var FAPCtx = new EF.CMSContext())
            {
                var _idea = FAPCtx.Idea.FirstOrDefault(c => c.Id == IdeaId);

                if (_idea != null)
                {
                    _idea.Views++;
                    FAPCtx.SaveChanges();
                    TempData["IdeaId"] = IdeaId;
                    return View(_idea);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> ChangePass()
        {
            var context = new CMSContext();
            var store = new UserStore<UserInfo>(context);
            var manager = new UserManager<UserInfo>(store);
            var user = await manager.FindByIdAsync(User.Identity.GetUserId());
            TempData["UserEmail"] = TempData["UserEmail"];
            TempData["UserId"] = TempData["UserId"];
            await CreateCode(user.Id);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePass(string oldpass, string newpass, string confirmpass, string verifycode)
        {
            var context = new CMSContext();
            var store = new UserStore<UserInfo>(context);
            var manager = new UserManager<UserInfo>(store);

            var user = await manager.FindByIdAsync(User.Identity.GetUserId());
            CustomValidationTrainee(oldpass, newpass, confirmpass, verifycode, user.Id);
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {

                if (user != null)
                {
                    var result = manager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, oldpass);
                    if (result == PasswordVerificationResult.Success)
                    {
                        String newPassword = newpass;
                        String hashedNewPassword = manager.PasswordHasher.HashPassword(newPassword);
                        user.PasswordHash = hashedNewPassword;
                        await store.UpdateAsync(user);
                        @TempData["alert"] = "Change PassWord successful";
                        return RedirectToAction("LogOut", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordHash", " Old Password incorrect!");
                        TempData["UserEmail"] = TempData["UserEmail"];
                        TempData["UserId"] = user.Id;
                        return View();
                    }

                }
                TempData["UserEmail"] = TempData["UserEmail"];
                TempData["UserId"] = user.Id;

                return RedirectToAction("Index", "Coor");
            }
        }

        public void CustomValidationTrainee(string ollpass, string newpass, string confirmpass, string verifycode, string userid)
        {
            if (string.IsNullOrEmpty(ollpass))
            {
                ModelState.AddModelError("PasswordHash", "Please input old Password");
            }
            if (string.IsNullOrEmpty(newpass))
            {
                ModelState.AddModelError("NewPass", "Please input new Password");
            }

            if (string.IsNullOrEmpty(confirmpass))
            {
                ModelState.AddModelError("PassConfirm", "Please input Confirm Password");
            }
            if (!string.IsNullOrEmpty(confirmpass) && !string.IsNullOrEmpty(newpass) && (confirmpass != newpass))
            {
                ModelState.AddModelError("PassConfirm", "New password and Confirm password not match");
            }
            if (!string.IsNullOrEmpty(confirmpass) && !string.IsNullOrEmpty(newpass) && (newpass.Length < 6))
            {
                ModelState.AddModelError("PassConfirm", "New password must longer than 5 character");
            }
            using (var database = new EF.CMSContext())
            {
                var code = database.FCode.Where(c => c.Code.ToString() == verifycode).FirstOrDefault();
                if (code == null)
                {
                    ModelState.AddModelError("VerifyCode", "Verify code not correct!");

                }
                if (code != null && code.UserId != userid)
                {
                    ModelState.AddModelError("VerifyCode", "Verify code not correct!");

                }
            }
        }

        public async Task CreateCode(string userid)
        {
            Random rnd = new Random();
            int Newcode = rnd.Next(100000, 999999);
            using (var database = new EF.CMSContext())
            {
                var code = database.FCode.Where(c => c.Code == Newcode).FirstOrDefault();
                if (code == null)
                {
                    var context = new CMSContext();
                    var store = new UserStore<UserInfo>(context);
                    var manager = new UserManager<UserInfo>(store);
                    var user = await manager.FindByIdAsync(User.Identity.GetUserId());
                    var newcode = new VerifyCode();
                    newcode.Code = Newcode;
                    newcode.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    newcode.UserId = userid;
                    database.FCode.Add(newcode);
                    database.SaveChanges();
                    await SendEmail(user.Email, "Your confirmation code is: " + newcode.Code.ToString() + "<p>This code available in 60 minute!</p>");
                }
                else { await CreateCode(userid); }

            }
        }
        public ActionResult ShowAllCategory()
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _category = dbCT.Category.ToList();
                return View(_category);
            }
        }
        public ActionResult Filter()
        {
            return View();
        }
        public ActionResult ShowComment(int IdeaId)
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _comment = dbCT.Comment
                                        .Where(c => c.IdeaId == IdeaId)
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
    }
}