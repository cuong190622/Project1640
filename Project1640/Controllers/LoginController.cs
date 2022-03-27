using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
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
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult LogIn()
        {
            CreateDate();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(UserInfo user)
        {
            if(user.Email == null || user.PasswordHash == null)
            {
                ModelState.AddModelError("PasswordHash", "Please input username and password!");
                return View();
            }
            var context = new CMSContext();
            var store = new UserStore<UserInfo>(context);
            var manager = new UserManager<UserInfo>(store);
            var signInManager
                   = new SignInManager<UserInfo, string>(manager, HttpContext.GetOwinContext().Authentication);

            var finder = await manager.FindByEmailAsync(user.Email);
            var result = await signInManager.PasswordSignInAsync(userName: user.Email.Split('@')[0], password: user.PasswordHash, isPersistent: false, shouldLockout: false);
            if (result == SignInStatus.Success)
            {
                var userStore = new UserStore<UserInfo>(context);
                var userManager = new UserManager<UserInfo>(userStore);

                if (await userManager.IsInRoleAsync(finder.Id, SecurityRoles.Admin))
                {
                    /*SessionLogin(fuser.UserName);*/
                    
                    return RedirectToAction("Index", "Admin");
                }
                if (await userManager.IsInRoleAsync(finder.Id, SecurityRoles.Staff))
                {
                    TempData["UserEmail"] = user.Email;
                    TempData["UserId"] = user.Id;
                    return RedirectToAction("Index", "Staff");
                }

                if (await userManager.IsInRoleAsync(finder.Id, SecurityRoles.Manager))
                {
                   
                    return RedirectToAction("Index", "Manager");
                }
                if (await userManager.IsInRoleAsync(finder.Id, SecurityRoles.Coor))
                {
                    
                    return RedirectToAction("Index", "Coor");
                }
                else return Content($"Comming Soon!!!");

                
            }
            else
            {
                ModelState.AddModelError("PasswordHash", "User Name or Password incorrect!");
                return View();
            }
        }

        public async Task<ActionResult> CreateRole(string email, string role) // tạo role cho identity 
        {
            var context = new CMSContext();
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<UserInfo>(context);
            var userManager = new UserManager<UserInfo>(userStore);

            if (!await roleManager.RoleExistsAsync(SecurityRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = SecurityRoles.Admin });
            }

            if (!await roleManager.RoleExistsAsync(SecurityRoles.Staff))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = SecurityRoles.Staff });
            }
            if (!await roleManager.RoleExistsAsync(SecurityRoles.Manager))
            {

                await roleManager.CreateAsync(new IdentityRole { Name = SecurityRoles.Manager });

            }
            if (!await roleManager.RoleExistsAsync(SecurityRoles.Coor))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = SecurityRoles.Coor });

            }

            var User = await userManager.FindByEmailAsync(email); // gán role cho user (thêm role ) 

            if (!await userManager.IsInRoleAsync(User.Id, SecurityRoles.Admin) && role == "admin")
            {
                userManager.AddToRole(User.Id, SecurityRoles.Admin);
            }
            if (!await userManager.IsInRoleAsync(User.Id, SecurityRoles.Staff) && role == "staff")
            {
                userManager.AddToRole(User.Id, SecurityRoles.Staff);
            }
            if (!await userManager.IsInRoleAsync(User.Id, SecurityRoles.Manager) && role == "manager")
            {
                userManager.AddToRole(User.Id, SecurityRoles.Manager);
            }
            if (!await userManager.IsInRoleAsync(User.Id, SecurityRoles.Coor) && role == "coor")
            {
                userManager.AddToRole(User.Id, SecurityRoles.Coor);
            }
            return Content("done!");
        }

        public void CreateDate()
        {
            var start = DateTime.Now.ToString("MM/dd/yyyy");
            var end = DateTime.Now.ToString("MM/dd/yyyy");
            using (var Database = new EF.CMSContext())
            {
                var FirstDate = Database.SetDate.Where(p => p.Id == 1).FirstOrDefault();
                if(FirstDate == null)
                {
                    var Date = new SetDate();
                    Date.StartDate = start;
                    Date.EndDate = end;
                    Database.SetDate.Add(Date);
                    Database.SaveChanges();
                }
            }
        }
        public async Task<ActionResult> CreateAdmin()
        {
            using (var Database = new EF.CMSContext())
            {
                var Department = new Department();
                Department.Name = "IT";
                Database.Department.Add(Department);
                Database.SaveChanges();
            }
            var context = new CMSContext();
            var store = new UserStore<UserInfo>(context);
            var manager = new UserManager<UserInfo>(store);

            var email = "cuong@gmail.com";
            var password = "123456@";
            var phone = "0961119526";
            var role = "admin";

            var user = await manager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new UserInfo
                {
                    UserName = email.Split('@')[0],
                    Email = email,
                    PhoneNumber = phone,
                    Name = email.Split('@')[0],
                    Role = role,
                    DepartmentId = 1
                };
                await manager.CreateAsync(user, password);
                await CreateRole(user.Email, "admin");
                return Content($"Create Admin account Succsess");
            }
            return RedirectToAction("LogIn");
        }
        public ActionResult LogOut() // function to Logout
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie); //Signout authentication cookie  
                ViewData.Clear(); // Clear All ViewData
                Session.RemoveAll(); // Clear All Session
            }
            return RedirectToAction("LogIn", "Login"); // Redirect user to login page
        }


    }

}

    
