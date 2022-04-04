using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using Project1640.EF;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;



namespace Project1640.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult Index()
        {
            using (var ctgrCt = new EF.CMSContext())
            {
                var categories = ctgrCt.Category
                                        .OrderBy(c => c.Id)
                                        .ToList();
                return View(categories);
            }
        }



        [HttpGet]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(Category a )
        {

            CustomValidationCategory(a);
            if (!ModelState.IsValid)
            {
                return View(a); // return lai Create.cshtml
                                    //di kem voi data ma user da go vao
            }
            else
            {

                using (var cate = new EF.CMSContext())
                {
                    if(a.Description == null)
                    {
                        a.Description = "No Description!";
                    }
                    cate.Category.Add(a);
                    cate.SaveChanges();
                }

                TempData["message"] = $"Successfully add class {a.Name} to system!";

                return RedirectToAction("Index");
            }



        }

        private void CustomValidationCategory(Category a)
        {
            if (string.IsNullOrEmpty(a.Description))
            {
                ModelState.AddModelError("Description", "Please input Description");
            }
        }

        [HttpGet]
        public ActionResult EditCategory(int id )
        {
            // lay category qua id tu db
            using(var cate = new EF.CMSContext())
            {
                var Category = cate.Category.FirstOrDefault(c => c.Id == id);
                return View(Category);
            }
        }

        [HttpPost]
        public ActionResult EditCategory(int id, Category a)
        {
            CustomValidationCategory(a);
            if (!ModelState.IsValid)
            {
                return View(a); // return lai Create.cshtml
                                //di kem voi data ma user da go vao
            }
            else {
                using (var cate = new EF.CMSContext())
                {
                    cate.Entry<Category>(a).State = System.Data.Entity.EntityState.Modified;

                    cate.SaveChanges();
                }

                return RedirectToAction("Index");
            }

              
        }

        [HttpGet]
        public ActionResult DeleteCategory(int id, Category a)
        {
            using (var cate = new EF.CMSContext())
            {
                var category = cate.Category.FirstOrDefault(c => c.Id == id);
                return View(category);
            }
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            using (var cate = new EF.CMSContext())
            {
                var Category = cate.Category.FirstOrDefault(b => b.Id == id);
                if (cate != null)
                {
                    cate.Category.Remove(Category);
                    cate.SaveChanges();
                }
                TempData["message"] = $"Successfully delete category with Id: {Category.Id}";
                return RedirectToAction("Index");
            }
        }
        //-----


        //public FileResult DownloadZipFile()
        //{
        //    var fileName = string.Format("{0} _Files.zip", DateTime.Today.Date.ToString("dd-MM-yyyy") + "_1");
        //    var tempOutPutPath = Server.MapPath(Url.Content("~/FileUpload/Allfile/")) + fileName;

        //    using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(tempOutPutPath)))
        //    {
        //        s.SetLevel(9); // 0-9, 9 being the highest compression  

        //        byte[] buffer = new byte[4096];

        //        var FileList = new List<string>();

        //        FileList.Add(Server.MapPath("~/FileUpload/Allfile/authors.txt"));
        //        // FileList.Add(Server.MapPath("~/FileUpload/authors.txt"));


        //        for (int i = 0; i < FileList.Count; i++)
        //        {
        //            ZipEntry entry = new ZipEntry(Path.GetFileName(FileList[i]));
        //            entry.DateTime = DateTime.Now;
        //            entry.IsUnicodeText = true;
        //            s.PutNextEntry(entry);

        //            using (FileStream fs = System.IO.File.OpenRead(FileList[i]))
        //            {
        //                int sourceBytes;
        //                do
        //                {
        //                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
        //                    s.Write(buffer, 0, sourceBytes);
        //                } while (sourceBytes > 0);
        //            }
        //        }
        //        s.Finish();
        //        s.Flush();
        //        s.Close();

        //        byte[] finalResult = System.IO.File.ReadAllBytes(tempOutPutPath);
        //        if (System.IO.File.Exists(tempOutPutPath))
        //            System.IO.File.Delete(tempOutPutPath);

        //        if (finalResult == null || !finalResult.Any())
        //            throw new Exception(String.Format("No Files found with Image"));

        //        return File(finalResult, "application/zip", fileName);

        //    }



        //}

        public ActionResult Download()
        {
            FileDownloads obj = new FileDownloads();

            //////int CurrentFileID = Convert.ToInt32(FileID);

            var filesCol = obj.GetFile().ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < filesCol.Count; i++)
                    {
                        ziparchive.CreateEntryFromFile(filesCol[i].FilePath, filesCol[i].FileName);

                    }
                }

                return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
            }
        }

        
        public ActionResult Csvfile()
        {
            CMSContext context = new CMSContext();
            var lstIdeas = (from Idea in context.Idea
                               select Idea);
            return View(lstIdeas);
        }


        [HttpPost]
        public FileResult Export()
        {
            CMSContext context = new CMSContext();
            List<object> lstIdeas = (from Idea in context.Idea.ToList().Take(10)
                                      select new[] { Idea.Id.ToString(),                                                                                                                    
                                                            Idea.Title,
                                                            Idea.Content
                                }).ToList<object>();

            //Insert the Column Names.
            lstIdeas.Insert(0, new string[3] { "Idea ID", "Idea Title", "Idea content" });

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lstIdeas.Count; i++)
            {
                string[] Ideas = (string[])lstIdeas[i];
                for (int j = 0; j < Ideas.Length; j++)
                {
                    //Append data with separator.
                    sb.Append(Ideas[j] + ',');
                }

                //Append new line character.
                sb.Append("\r\n");

            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Grid.csv");
        
        }

        public ActionResult Chart(string year = "2022")
        {
            int number = 0;
            if (Regex.IsMatch(year, @"^\d+$") && Int32.Parse(year) > 0)
            {
                number = Int32.Parse(year);
            }
            using (CMSContext context = new CMSContext()) //create a connection with the database
            {
                var ideaDepartment = (
                   from d in context.Department
                   join u in context.Users on d.Id equals u.DepartmentId
                   join i in context.Idea on u.Id equals i.UserId
                   select new
                   {
                       name = d.Name,
                       year = i.Date 
                   }).Where(p => p.year.Contains(year)).ToList();
                if(ideaDepartment.Count() == 0)
                {
                    List<DataPoint> dataPoints = new List<DataPoint>();
                    foreach (var a in ListDepartment())
                    {
                        dataPoints.Add(new DataPoint(a.Name, CountIdeaPerDepartment(a.Name, 0)));
                    }
                    ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
                    TempData["Sub"] = "No data Idea per department in year" + year + "|| All data will be shown!";
                    return View();
                }
                else
                {
                    List<DataPoint> dataPoints = new List<DataPoint>();
                    foreach (var a in ListDepartment())
                    {
                        dataPoints.Add(new DataPoint(a.Name, CountIdeaPerDepartment(a.Name, number)));
                    }
                    ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
                    TempData["Sub"] = "Idea per department in year" + year;
                    return View();
                }
            }
            
        }
        public List<ListDepartment> ListDepartment()
        {
            using (CMSContext context = new CMSContext()) //create a connection with the database
            {
                var Department = (from d in context.Department select new ListDepartment { Name = d.Name }).ToList();
                return Department;
            }
        }

        public int CountIdeaPerDepartment(string department, int year)
        {
            if (year !=0 )
            {
                using (CMSContext context = new CMSContext()) //create a connection with the database
                {
                    var ideaDepartment = (
                       from d in context.Department
                       join u in context.Users on d.Id equals u.DepartmentId
                       join i in context.Idea on u.Id equals i.UserId
                       select new
                       {
                           name = d.Name,
                           year = i.Date
                       }).Where(p => p.name == department).Where(p => p.year.Contains(year.ToString())).ToList();
                    return ideaDepartment.Count();
                }
            }
            else
            {
                using (CMSContext context = new CMSContext()) //create a connection with the database
                {
                    var ideaDepartment = (
                       from d in context.Department
                       join u in context.Users on d.Id equals u.DepartmentId
                       join i in context.Idea on u.Id equals i.UserId
                       select new
                       {
                           name = d.Name,
                           year = i.Date
                       }).Where(p => p.name == department).ToList();
                    return ideaDepartment.Count();
                }
            }
            
        }

        public void Statitisc()
        {
            using(var database =  new EF.CMSContext())
            {
              //  var 
            }
        }
        public ActionResult FliterYear()
        {
            return View();
        }
        public ActionResult ViewAllIdea(int id = 1, int categoryId = 0, string count = "a")
        {
            int number = 5;
            if (Regex.IsMatch(count, @"^\d+$") && Int32.Parse(count) > 0)
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
                    return RedirectToAction("ViewAllIdea");
                }
            }
        }
        [HttpGet]
        public ActionResult ShowAllCategory()
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _category = dbCT.Category.ToList();
                return View(_category);
            }
        }
        [HttpGet]
        public ActionResult Filter()
        {
            return View();
        }
        [HttpGet]
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
        public ActionResult TopView()
        {
            using (var dbCT = new EF.CMSContext())
            {
                try
                {
                    var _idea = dbCT.Idea.OrderByDescending(c => c.Views).First();
                    return RedirectToAction("ViewIdea", new { IdeaId = _idea.Id });
                }
                catch (Exception)
                {
                    TempData["alert"] = $"No ideas at the moment, please try again later!!";
                    return RedirectToAction("ViewAllIdea");
                }
            }

        }
        public ActionResult TopLike()
        {
            using (var dbCT = new EF.CMSContext())
            {
                try
                {
                    var _idea = dbCT.Idea.OrderByDescending(c => c.Rank).First();
                    return RedirectToAction("ViewIdea", new { IdeaId = _idea.Id });
                }
                catch (Exception)
                {
                    TempData["alert"] = $"No ideas at the moment, please try again later!!";
                    return RedirectToAction("ViewAllIdea");
                }
            }

        }

        public ActionResult LastIdea()
        {
            using (var dbCT = new EF.CMSContext())
            {

                try
                {
                    var _idea = dbCT.Idea.OrderByDescending(c => c.Id).First();
                    return RedirectToAction("ViewIdea", new { IdeaId = _idea.Id });
                }
                catch (Exception)
                {
                    TempData["alert"] = $"No ideas at the moment, please try again later!!";
                    return RedirectToAction("ViewAllIdea");
                }
            }
        }
        [HttpGet]
        public ActionResult LastComment()
        {
            using (var dbCT = new EF.CMSContext())
            {
                
                try
                {
                    var _comment = dbCT.Comment.OrderByDescending(c => c.Id).First();
                    TempData["LastComment"] = _comment.Id;
                    return RedirectToAction("ViewIdea", new { IdeaId = _comment.IdeaId });
                }
                catch (Exception)
                {
                    TempData["alert"] = $"No Comments at the moment, please try again later!!";
                    return RedirectToAction("ViewAllIdea");
                }
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
    }

}
