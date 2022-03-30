
using Microsoft.AspNet.Identity;
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

        public ActionResult Index(int id = 1, int categoryId = 0)
        {
            if(categoryId == 0)
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
            else
            {
                using (var dbCT = new EF.CMSContext())
                {
                    TempData["CategoryId"] = categoryId;
                    int Count = dbCT.Idea.Where(p => p.CategoryId == categoryId).Count();
                    if (Count <= 5)
                    {
                        TempData["PageNo"] = 1;
                        TempData["PageMax"] = 1;
                        var ideas = dbCT.Idea.Where(p => p.CategoryId == categoryId).OrderBy(c => c.Id).ToList();
                        return View(ideas);
                    }
                    else
                    {
                        var ideas = dbCT.Idea.Where(p => p.CategoryId == categoryId).OrderBy(c => c.Id).ToList();
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
            if (CheckDate())
            {
                ViewBag.Class = getList();
                return View();
            }
            else
            {
                return RedirectToAction("BlockTime");
            }
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
        public async Task<ActionResult> CreateIdea(Idea a, FormCollection f, HttpPostedFileBase postedFile)
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
                var store = new UserStore<UserInfo>(context);
                var manager = new UserManager<UserInfo>(store);
                var user = await manager.FindByIdAsync(User.Identity.GetUserId());
                using (var Database = new EF.CMSContext())
                {
                    a.Status = !a.Status;
                    a.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    a.UserId = user.Id;
                    Database.Idea.Add(a);
                    Database.SaveChanges();
                    await SaveFile( new FileUpload(), postedFile, a.Id);

                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task SaveFile(FileUpload a, HttpPostedFileBase postedFile, int IdeaId)
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
                    a.UserId = await FindIdUserByEmail(TempData["UserEmail"].ToString());
                    a.IdeaId = IdeaId;
                    Database.File.Add(a);
                    Database.SaveChanges();
                }
            }

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
        [HttpGet]
        public ActionResult CreateComment(int IdeaId)
        {
                TempData["IdeaId"] = IdeaId;
                return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateComment(Comment a)
        {
            if (!CheckDate())
            {
                return RedirectToAction("BlockTime");
            }
            var context = new CMSContext();
            var store = new UserStore<UserInfo>(context);
            var manager = new UserManager<UserInfo>(store);
            var user = await manager.FindByIdAsync(User.Identity.GetUserId());
            using (var database = new EF.CMSContext())
            {
                a.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                a.UserId = user.Id;
                a.Status = !a.Status;
                database.Comment.Add(a);
                database.SaveChanges();
                await SendEmail(FindEmailUserByCommentId(a.Id), "New user comment on your post! <p>Comment: " + a.Content + " </p>");
                TempData["IdeaId"] = a.IdeaId;
            }
            return RedirectToAction("ViewIdea", new { IdeaId = a.IdeaId });
        }
        public ActionResult ShowCategory(int CategoryId)
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _category = dbCT.Category
                                        .Where(c => c.Id == CategoryId)
                                        .ToList();
             return View(_category);
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

        [HttpGet]       
        public ActionResult  Like()
        {
            var context = new CMSContext();
            var store = new UserStore<UserInfo>(context);
            var manager = new UserManager<UserInfo>(store);
            var user =  manager.FindById(User.Identity.GetUserId());
            Getlike(Int32.Parse(TempData["IdeaId"].ToString()), user.Id);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Like(React a, string like)
        {            
            using (var database = new EF.CMSContext())
            {
                var context = new CMSContext();
                var store = new UserStore<UserInfo>(context);
                var manager = new UserManager<UserInfo>(store);
                var user = await manager.FindByIdAsync(User.Identity.GetUserId());
                int react = database.React.Where(c => c.IdeaId == a.IdeaId).Count();
                if(react != 0)
                {
                    var listLiked = database.React.Where(c => c.IdeaId == a.IdeaId).Where(c => c.UserId == user.Id).ToList();
                    database.React.RemoveRange(listLiked);
                }
                if(like == "Up")
                {
                    a.React_Type = true;
                }else a.React_Type = false;
                a.UserId = user.Id;
                database.React.Add(a);
                database.SaveChanges();
                TempData["IdeaId"] = a.IdeaId;
            }
            return RedirectToAction("ViewIdea", new { IdeaId = a.IdeaId });
        }
        
        public void Getlike(int IdeaID, string User)
        {
            using (var dbCT = new EF.CMSContext())
            {
                int like = dbCT.React.Where(c => c.IdeaId == IdeaID).Where(c => c.React_Type == true).Count();
                int dislike = dbCT.React.Where(c => c.IdeaId == IdeaID).Where(c => c.React_Type == false).Count();
                TempData["LikeCount"] = like - dislike;
                int Userlike = dbCT.React.Where(c => c.IdeaId == IdeaID).Where(c => c.React_Type == true).Where(c => c.UserId == User).Count();
                int Userdislike = dbCT.React.Where(c => c.IdeaId == IdeaID).Where(c => c.React_Type == false).Where(c => c.UserId == User).Count();
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
                    database.SaveChanges();
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
        public string FindEmailUserByCommentId(int commentId)
        {
            using (CMSContext context = new CMSContext()) //create a connection with the database
            {
                var email = (                    
                    from c in context.Comment
                    join i in context.Idea on c.IdeaId equals i.Id
                    join u in context.Users on i.UserId equals u.Id
                    select new {
                        Id = c.Id,
                        Email = u.Email
                    }).Where(p => p.Id == commentId).First();
                return email.Email.ToString();
            }            
        }

        public ActionResult ShowFile(int IdeaId)
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _files = dbCT.File
                                        .Where(c => c.IdeaId == IdeaId)
                                        .ToList();
                return View(_files);
            }
        }

        public bool CheckDate()
        {
            using (var Database = new EF.CMSContext())
            {
                var FirstDate = Database.SetDate.Where(p => p.Id == 1).FirstOrDefault();
                if (FirstDate != null)
                {
                    if(Int32.Parse(FirstDate.StartDate.Split('-')[0]) <= Int32.Parse(DateTime.Now.ToString("yyyy")) && Int32.Parse(DateTime.Now.ToString("yyyy")) <= Int32.Parse(FirstDate.EndDate.Split('-')[0]))
                    {
                        if (Int32.Parse(FirstDate.StartDate.Split('-')[1]) <= Int32.Parse(DateTime.Now.ToString("MM")) && Int32.Parse(DateTime.Now.ToString("MM")) <= Int32.Parse(FirstDate.EndDate.Split('-')[1]))
                        {
                            if (Int32.Parse(FirstDate.StartDate.Split('-')[2]) <= Int32.Parse(DateTime.Now.ToString("dd")) && Int32.Parse(DateTime.Now.ToString("dd")) <= Int32.Parse(FirstDate.EndDate.Split('-')[2]))
                            {
                                return true;
                            }
                        }
                        if (Int32.Parse(FirstDate.StartDate.Split('-')[1]) <= Int32.Parse(DateTime.Now.ToString("MM")) && Int32.Parse(DateTime.Now.ToString("MM")) < Int32.Parse(FirstDate.EndDate.Split('-')[1]))
                        {
                            return true;
                        }
                    }

                }
                return false;
            }
        }
        [HttpGet]
        public async Task<ActionResult>  ChangePass()
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

                return RedirectToAction("Index", "Staff");
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
            using(var database = new EF.CMSContext())
            {
                var code = database.FCode.Where(c => c.Code.ToString() == verifycode).FirstOrDefault();
                if(code == null)
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
            using(var database = new EF.CMSContext())
            {
                var code = database.FCode.Where(c => c.Code == Newcode).FirstOrDefault();
                if(code == null)
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
        public ActionResult BlockTime()
        {
            return View();
        }
        public ActionResult ShowAllCategory()
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _category = dbCT.Category.ToList();
                return View(_category);
            }
        }
    }
}