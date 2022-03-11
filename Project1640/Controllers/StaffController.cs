using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Project1640.EF;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Project1640.Controllers
{
    public class StaffController : Controller
    {
        public ActionResult Index()
        {
            using (var ct = new EF.CMSContext())
            {
                var categories = ct.Idea
                                        .OrderBy(c => c.Id)
                                        .ToList();
                return View(categories);
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
                    a.Status = true;
                    a.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    
                    a.UserId = "bdb22222-180f-4f4d-a179-127d681c48b0";
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
                    a.UserId = "bdb22222-180f-4f4d-a179-127d681c48b0";
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
        public ActionResult CreateComment(Comment a, int id)
        {
            using (var database = new EF.CMSContext())
            {
                a.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                a.UserId = "bdb22222-180f-4f4d-a179-127d681c48b0";
                a.IdeaId = id;
                database.Comment.Add(a);
                database.SaveChanges();
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
    }
}