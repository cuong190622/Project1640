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
            return View();
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

        [HttpGet]
        public ActionResult CreateIdea()
        {
            SetViewBag();
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
                    a.listCategory = Convert(context, f["formatIds[]"]);
                    a.UserId = "d354a0e4-55a1-43ce-b20c-82d8211729ea";
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
                    a.UserId = "d354a0e4-55a1-43ce-b20c-82d8211729ea";
                    a.IdeaId = IdeaId;
                    Database.File.Add(a);
                    Database.SaveChanges();
                }
            }

        }

        public ActionResult ViewAllIdea()
        {
            return View();
        }


    }
}