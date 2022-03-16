using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Project1640.EF;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Project1640.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            using (var ct = new EF.CMSContext())
            {
                var user = ct.Users.OrderBy(a => a.Id).ToList();
                return View(user);
            }
        }
        [HttpGet]
        public ActionResult CreateAccount()
        {
            ViewBag.Class = getList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount(UserInfo newUser)
        {
            CMSContext context = new CMSContext();
            var roleManager = new Microsoft.AspNet.Identity.RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new Microsoft.AspNet.Identity.UserManager<UserInfo>(new UserStore<UserInfo>(context));
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }
            else
            {
                var user = new UserInfo
                {
                    UserName = newUser.Email.Split('@')[0],
                    Email = newUser.Email,
                    Age = newUser.Age,
                    WorkingPlace = newUser.WorkingPlace,
                    DoB = newUser.DoB,
                    Name = newUser.Name,
                    Role = newUser.Role,
                    PasswordHash = newUser.PasswordHash,
                    DepartmentId = newUser.DepartmentId

                };
                //validate email
                if (user.Email == null)
                {
                    ModelState.AddModelError("Gmail", "Email cannot be null ");
                    ViewBag.Class = getList();
                    return View(newUser);
                }
                else
                {
                    var ct = new CMSContext();
                    var a = ct.Users.FirstOrDefault(t => t.Email == user.Email);
                    //check email null
                    if (a == null)
                    {
                        var result = await userManager.CreateAsync(user, "Xyz@12345");
                    }
                    else
                    {
                        ModelState.AddModelError("Gmail", "This Email already exists  ");
                        ViewBag.Class = getList();
                        return View(newUser);
                    }
                }
                return RedirectToAction("Index");
            }
        }
        private dynamic getList()
        {
            using (var abc = new EF.CMSContext()) //create a new value abc is an object of CMSContext
            {
                var stx = abc.Department.Select(p => new SelectListItem //Select anonymous
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();

                return stx;
            }
        }

        [HttpGet]
        public ActionResult EditAccount(string id)
        {
            CMSContext context = new CMSContext();
            var roleManager = new Microsoft.AspNet.Identity.RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new Microsoft.AspNet.Identity.UserManager<UserInfo>(new UserStore<UserInfo>(context));
            using (var bwCtx = new CMSContext())
            {
                var ct = bwCtx.Users.FirstOrDefault(t => t.Id == id);
                //ef method to select only one or null if not found

                if (ct != null) // if a book is found, show edit view
                {

                    return View(ct);
                }
                else // if no book is found, back to index
                {

                    return RedirectToAction("Index"); //redirect to action in the same controller
                }
            }
        }

        [HttpPost]
        public ActionResult EditAccount(string id, UserInfo newUser)
        {
            CMSContext context = new CMSContext();
            var roleManager = new Microsoft.AspNet.Identity.RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new Microsoft.AspNet.Identity.UserManager<UserInfo>(new UserStore<UserInfo>(context));

            using (var ct = new CMSContext())
            {
                ct.Entry<UserInfo>(newUser).State
                    = System.Data.Entity.EntityState.Modified;
                ct.SaveChanges();
            }
            TempData["message"] = $"Successfully update book with Id:{newUser.Id} ";
            return RedirectToAction("Index");
        }

        public ActionResult deleteAccount(string id)
        {
            using (var ct = new CMSContext())
            {
                var newUser = ct.Users.FirstOrDefault(b => b.Id == id);

                if (newUser != null)
                {
                    ct.Users.Remove(newUser);
                    ct.SaveChanges();
                    TempData["message"] = $"Successfully delete book with Id: {newUser.Id}";
                }


                return RedirectToAction("Index");
            }
        }

        private List<Department> Convert(EF.CMSContext database, string formatIds)
        {
            if (formatIds != null)
            {
                var abc = formatIds.Split(',').Select(id => Int32.Parse(id)).ToArray();
                return database.Department.Where(f => abc.Contains(f.Id)).ToList();
            }
            else
            {
                return database.Department.Where(c => c.Id == 0).ToList();
            }
        }
        private void SetViewBag()
        {
            using (var bwCtx = new EF.CMSContext())// use a variable named bwCtx of CMSContext 
            {
                ViewBag.Formats = bwCtx.Department.ToList(); //select all data from Courses in DbSet
            }
        }


        public ActionResult ShowDepartment(int id)
        {

            using (var dpm = new EF.CMSContext())
            {
                var _department = dpm.Department
                                        .Where(c => c.Id == id)
                                        .ToList();
                return View(_department);
            }
        }

       


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult IndexDepartment()
        {
            using (var dpm = new EF.CMSContext())
            {
                var department = dpm.Department
                                        .OrderBy(c => c.Id)
                                        .ToList();
                return View(department);
            }
        }

        // create Department and view
        [HttpGet]
        public ActionResult CreateDepartment()
        {
            ViewBag.Class = getList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateDepartment(Department a)
        {
            using (var dpm = new EF.CMSContext())
            {
                dpm.Department.Add(a);
                dpm.SaveChanges();
            }

            TempData["message"] = $"Successfully add class {a.Name} to system!";

            return RedirectToAction("IndexDepartment");
        }


        [HttpGet]
        public ActionResult EditDepartment(int id)
        {
            // lay category qua id tu db
            using (var dpm = new EF.CMSContext())
            {
                var Department = dpm.Department.FirstOrDefault(c => c.Id == id);
                return View(Department);
            }
        }

        [HttpPost]
        public ActionResult EditDepartment(int id, Department a)
        {
            using (var dpm = new EF.CMSContext())
            {
                dpm.Entry<Department>(a).State = System.Data.Entity.EntityState.Modified;

                dpm.SaveChanges();
            }

            return RedirectToAction("IndexDepartment");
        }


        [HttpGet]
        public ActionResult DeleteDepartment(int id, Department a)
        {
            using (var dpm = new EF.CMSContext())
            {
                var department = dpm.Department.FirstOrDefault(c => c.Id == id);
                return View(department);
            }
        }


        [HttpPost]
        public ActionResult DeleteDepartment(int id)
        {
            using (var dpm = new EF.CMSContext())
            {
                var Department = dpm.Department.FirstOrDefault(b => b.Id == id);
                if (dpm != null)
                {
                    dpm.Department.Remove(Department);
                    dpm.SaveChanges();
                }
                TempData["message"] = $"Successfully delete book with Id: {Department.Id}";
                return RedirectToAction("IndexDepartment");
            }
        }


    }
}
