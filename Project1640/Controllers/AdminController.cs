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
                    Name = newUser.Name,
                    Role = "trainer",
                    PasswordHash = "123qwe123",
                    DepartmentId = 1
                };
                //validate email
                if (user.Email == null)
                {
                    ModelState.AddModelError("Gmail", "Email cannot be null ");
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
                        if (result.Succeeded)
                        {
                            userManager.AddToRole(user.Id, newUser.Role);

                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Gmail", "This Email already exists  ");
                        return View(newUser);
                    }
                }
                return RedirectToAction("Index");
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
    }
}
