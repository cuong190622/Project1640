using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Project1640.EF;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Project1640.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [Authorize(Roles = SecurityRoles.Admin)]
        public ActionResult Index()
        {
            using (var ct = new EF.CMSContext())
            {
                var user = ct.Users.Where(a => a.Role != "admin").OrderBy(a => a.Id).ToList();
                return View(user);
            }
        }
        [HttpGet]
        [Authorize(Roles = SecurityRoles.Admin)]
        public ActionResult Createstaff()
        {
            ViewBag.Class = getList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Createstaff(UserInfo staff)
        {

            CustomValidationStaff(staff);

            if (!ModelState.IsValid)
            {
                ViewBag.Class = getList();
                TempData["alert"] = $"Create account Fail, data input not allowed! Try again!";
                return RedirectToAction("Index");
            }
            else
            {
                var context = new CMSContext();
                var store = new UserStore<UserInfo>(context);
                var manager = new UserManager<UserInfo>(store);

                var user = await manager.FindByEmailAsync(staff.Email);
                
                if (user == null)
                {
                    user = new UserInfo
                    {
                        UserName = staff.Email.Split('@')[0],
                        Email = staff.Email,
                        Age = staff.Age,
                        WorkingPlace = staff.WorkingPlace,
                        DoB = staff.DoB,
                        DepartmentId = staff.DepartmentId,
                        Role = "staff",
                        PhoneNumber = staff.PhoneNumber,
                        PasswordHash = "123qwe123",
                        Name = staff.Name
                    };
                    await manager.CreateAsync(user, user.PasswordHash);
                    await CreateRole(staff.Email, "staff");
                    TempData["message"] = $"Account successfully created!";
                }
                else
                {
                    TempData["alert"] = $"Email already exists, please try again!!";
                }
                return RedirectToAction("Index");
            }


        }

        public void CustomValidationStaff(UserInfo staff)
        {
            if (string.IsNullOrEmpty(staff.Email))
            {
                ModelState.AddModelError("Email", "Please input Email");
            }
        }
        [Authorize(Roles = SecurityRoles.Admin)]
        public ActionResult ViewAccount(string id)
        {
            CMSContext context = new CMSContext();
            var roleManager = new Microsoft.AspNet.Identity.RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new Microsoft.AspNet.Identity.UserManager<UserInfo>(new UserStore<UserInfo>(context));
            using (var bwCtx = new CMSContext())
            {
                ViewBag.Class = getList();
                var ct = bwCtx.Users.FirstOrDefault(t => t.Id == id);
                return View(ct);
            }
        }
        [Authorize(Roles = SecurityRoles.Admin)]
        [HttpGet]
        public ActionResult CreateManager()
        {
            ViewBag.Class = getList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateManager(UserInfo mana)
        {
            CustomValidationManager(mana);

            if (!ModelState.IsValid)
            {
                ViewBag.Class = getList();
                TempData["alert"] = $"Create account Fail, data input not allowed! Try again!";
                return RedirectToAction("Index");
            }
            else
            {
                var context = new CMSContext();
                var store = new UserStore<UserInfo>(context);
                var manager = new UserManager<UserInfo>(store);

                var user = await manager.FindByEmailAsync(mana.Email);
                
                if (user == null)
                {
                    user = new UserInfo
                    {
                        UserName = mana.Email.Split('@')[0],
                        Email = mana.Email,
                        Age = mana.Age,
                        WorkingPlace = mana.WorkingPlace,
                        DoB = mana.DoB,
                        DepartmentId = mana.DepartmentId,
                        Role = "manager",
                        PasswordHash = "123qwe123",
                        PhoneNumber = mana.PhoneNumber,
                        Name = mana.Name
                    };
                    await manager.CreateAsync(user, user.PasswordHash);
                    await CreateRole(mana.Email, "manager");
                    TempData["message"] = $"Account successfully created!";
                }
                else
                {
                    TempData["alert"] = $"Email already exists, please try again!!";
                }

                return RedirectToAction("Index");
            }


        }

        public void CustomValidationManager(UserInfo mana)
        {
            if (string.IsNullOrEmpty(mana.Email))
            {
                ModelState.AddModelError("Email", "Please input Email");
            }
        }

        [Authorize(Roles = SecurityRoles.Admin)]
        [HttpGet]
        public ActionResult CreateCoor()
        {
            ViewBag.Class = getList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCoor(UserInfo coor)
        {
            CustomValidationCoor(coor);

            if (!ModelState.IsValid)
            {
                ViewBag.Class = getList();
                TempData["alert"] = $"Create account Fail, data input not allowed! Try again!";
                return RedirectToAction("Index");
            }
            else
            {
                var context = new CMSContext();
                var store = new UserStore<UserInfo>(context);
                var manager = new UserManager<UserInfo>(store);

                var user = await manager.FindByEmailAsync(coor.Email);
                
                if (user == null)
                {
                    user = new UserInfo
                    {
                        UserName = coor.Email.Split('@')[0],
                        Email = coor.Email,
                        Age = coor.Age,
                        WorkingPlace = coor.WorkingPlace,
                        DoB = coor.DoB,
                        DepartmentId = coor.DepartmentId,
                        Role = "coor",
                        PasswordHash = "123qwe123",
                        PhoneNumber = coor.PhoneNumber,
                        Name = coor.Name
                    };
                    await manager.CreateAsync(user, user.PasswordHash);
                    await CreateRole(coor.Email, "coor");
                    TempData["message"] = $"Account successfully created!";
                }
                else
                {
                    TempData["alert"] = $"Email already exists, please try again!!";
                }
                
                return RedirectToAction("Index");
            }


        }

        public void CustomValidationCoor(UserInfo coor)
        {
            if (string.IsNullOrEmpty(coor.Email))
            {
                ModelState.AddModelError("Email", "Please input Email");
            }
        }


        // #####################################################

        //CMSContext context = new CMSContext();
        //var roleManager = new Microsoft.AspNet.Identity.RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        //var userManager = new Microsoft.AspNet.Identity.UserManager<UserInfo>(new UserStore<UserInfo>(context));
        //if (!ModelState.IsValid)
        //{
        //    return View(newUser);
        //}
        //else
        //{
        //    var user = new UserInfo
        //    {
        //        UserName = newUser.Email.Split('@')[0],
        //        Email = newUser.Email,
        //        Age = newUser.Age,
        //        WorkingPlace = newUser.WorkingPlace,
        //        DoB = newUser.DoB,
        //        Name = newUser.Name,

        //        PasswordHash = newUser.PasswordHash,
        //        DepartmentId = newUser.DepartmentId

        //    };

        //await userManager.CreateAsync(user, PasswordHash);
        //await CreateRole(user.Email, "admin");
        //return Content($"Create Admin account Succsess")
        //validate email
        //if (user.Email == null)
        //{
        //    ModelState.AddModelError("Gmail", "Email cannot be null ");
        //    ViewBag.Class = getList();
        //    return View(newUser);
        //}
        //else
        //{
        //    var ct = new CMSContext();
        //    var a = ct.Users.FirstOrDefault(t => t.Email == user.Email);
        //    //check email null
        //    if (a == null)
        //    {
        //        var result = await userManager.CreateAsync(user, "Xyz@12345");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("Gmail", "This Email already exists  ");
        //        ViewBag.Class = getList();
        //        return View(newUser);
        //    }
        //}
        //    return RedirectToAction("Index");
        //}

        public List<SelectListItem> getList()
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
        [Authorize(Roles = SecurityRoles.Admin)]
        [HttpGet]
        public ActionResult EditAccount(string id)
        {
            CMSContext context = new CMSContext();
            var roleManager = new Microsoft.AspNet.Identity.RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new Microsoft.AspNet.Identity.UserManager<UserInfo>(new UserStore<UserInfo>(context));
            using (var bwCtx = new CMSContext())
            {
                ViewBag.Class = getList();
                var ct = bwCtx.Users.FirstOrDefault(t => t.Id == id);
                if (ct != null) 
                {
                    return View(ct);
                }
                else 
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
            TempData["message"] = $"Successfully update account :{newUser.Email} ";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = SecurityRoles.Admin)]
        public ActionResult deleteAccount(string id)
        {
            using (var ct = new CMSContext())
            {
                var newUser = ct.Users.FirstOrDefault(b => b.Id == id);

                if (newUser != null)
                {
                    ct.Users.Remove(newUser);
                    ct.SaveChanges();
                    TempData["message"] = $"Successfully delete Account: {newUser.Email}";
                }


                return RedirectToAction("Index");
            }
        }

        public List<Department> Convert(EF.CMSContext database, string formatIds)
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
        public void SetViewBag()
        {
            using (var bwCtx = new EF.CMSContext())// use a variable named bwCtx of CMSContext 
            {
                ViewBag.Formats = bwCtx.Department.ToList(); //select all data from Courses in DbSet
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




        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [Authorize(Roles = SecurityRoles.Admin)]
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
        [Authorize(Roles = SecurityRoles.Admin)]

        [HttpGet]
        public ActionResult CreateDepartment()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateDepartment(Department a)
        {
            using (var dpm = new EF.CMSContext())
            {
                try
                {
                    dpm.Department.Add(a);
                    dpm.SaveChanges();
                }
                catch (Exception)
                {
                    TempData["alert"] = $"Add Department Fail! data input not allowed! try again!!";
                    return RedirectToAction("IndexDepartment");
                }
               
            }

            TempData["message"] = $"Successfully add department {a.Name} to system!";

            return RedirectToAction("IndexDepartment");
        }

        [Authorize(Roles = SecurityRoles.Admin)]
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
                try
                {
                    dpm.Entry<Department>(a).State = System.Data.Entity.EntityState.Modified;
                    dpm.SaveChanges();
                }
                catch (Exception)
                {
                    TempData["alert"] = $"Edit Department Fail! data input not allowed! try again!!";
                    return RedirectToAction("IndexDepartment");
                }
               
            }
            TempData["message"] = $"Update department {a.Name} Successfully!";
            return RedirectToAction("IndexDepartment");
        }

        [Authorize(Roles = SecurityRoles.Admin)]
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
            string a = User.Identity.GetUserId();
            using (var data = new EF.CMSContext())
            {
                var ex = (from c in data.Department
                          join i in data.Users on c.Id equals i.DepartmentId
                          select new
                          {
                              id = i.Id
                          }).Where(p => p.id == a).Count();
                if (ex != 0)
                {
                    TempData["alert"] = $"Can not delete this Department!";
                    return RedirectToAction("Index");
                }
            }
            using (var dpm = new EF.CMSContext())
            {
                var Department = dpm.Department.FirstOrDefault(b => b.Id == id);
                if (dpm != null)
                {
                    dpm.Department.Remove(Department);
                    dpm.SaveChanges();
                }
                TempData["message"] = $"Successfully delete department with Id: {Department.Id}";
                return RedirectToAction("IndexDepartment");
            }
        }

        /// ////////////////////////////////////////////////////
        [Authorize(Roles = SecurityRoles.Admin)]
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

/*        public ActionResult LastComment()
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
                    TempData["alert"] = $"No Comment at the moment, please try again later!!";
                    return RedirectToAction("Index");
                }

            }
        }*/

        public ActionResult Top5Idea(string count = "5")
        {
            int number = 5;
            if (Regex.IsMatch(count, @"^\d+$") && Int32.Parse(count) > 0)
            {
                number = Int32.Parse(count);
            }
            using (var dbCT = new EF.CMSContext())
            {
                var _ideas = dbCT.Idea.OrderByDescending(c => c.Views).Take(number).ToList();
                TempData["Number"] = number;
                return View(_ideas);
            }
        }
        public ActionResult Top5Like(string count = "5")
        {
            int number = 5;
            if (Regex.IsMatch(count, @"^\d+$") && Int32.Parse(count) > 0)
            {
                number = Int32.Parse(count);
            }
            using (var dbCT = new EF.CMSContext())
            {
                var _ideas = dbCT.Idea.OrderByDescending(c => c.Rank).Take(number).ToList();
                TempData["Number"] = number;
                return View(_ideas);
            }
        }
        [Authorize(Roles = SecurityRoles.Admin)]
        [HttpGet]
        public ActionResult EditSetDate(int id = 1)
        {
            using (var sd = new EF.CMSContext())
            {

                var setdate = sd.SetDate.FirstOrDefault(c => c.Id == id);
                return View(setdate);
            }
        }

        //HTTPOST CREATE EDITSETDATE
        [HttpPost]
        public ActionResult EditSetDate(SetDate a)
        {
            using (var sd = new EF.CMSContext())
            {
                var setdate = sd.SetDate.FirstOrDefault(c => c.Id == 1);
                setdate.StartDate = a.StartDate;
                setdate.EndDate = a.EndDate;
                sd.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        [Authorize(Roles = SecurityRoles.Admin)]
        public ActionResult IndexCategory()
        {
            using (var ctgrCt = new EF.CMSContext())
            {
                var categories = ctgrCt.Category
                                        .OrderBy(c => c.Id)
                                        .ToList();
                return View(categories);
            }
        }


        [Authorize(Roles = SecurityRoles.Admin)]
        [HttpGet]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(Category a)
        {
            using (var cate = new EF.CMSContext())
            {
                try
                {
                    cate.Category.Add(a);
                    cate.SaveChanges();
                }
                catch (Exception)
                {
                    TempData["alert"] = $"Add category Fail! data input not allowed! try again!!";
                    return RedirectToAction("IndexCategory");
                }
                
            }

            TempData["message"] = $"Successfully add category {a.Name} to system!";

            return RedirectToAction("IndexCategory");
        }

        [Authorize(Roles = SecurityRoles.Admin)]
        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            // lay category qua id tu db
            using (var cate = new EF.CMSContext())
            {
                var Category = cate.Category.FirstOrDefault(c => c.Id == id);
                return View(Category);
            }
        }

        [HttpPost]
        public ActionResult EditCategory(int id, Category a)
        {
            using (var cate = new EF.CMSContext())
            {
                try
                {
                    cate.Entry<Category>(a).State = System.Data.Entity.EntityState.Modified;
                    cate.SaveChanges();
                }
                catch(Exception)
                {
                    TempData["alert"] = $"Edit category Fail! data input not allowed! try again!!";
                    return RedirectToAction("IndexCategory");
                }

            }
            TempData["message"] = $"Update category {a.Name} Successfully!";
            return RedirectToAction("IndexCategory");
        }
        [Authorize(Roles = SecurityRoles.Admin)]
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
                TempData["message"] = $"Successfully delete Category with Name: {Category.Name}";
                return RedirectToAction("IndexCategory");
            }
        }
        public ActionResult Filter()
        {
            return View();
        }
        public ActionResult Filter2()
        {
            return View();
        }
        public ActionResult Filter3()
        {
            return View();
        }
        [Authorize(Roles = SecurityRoles.Admin)]
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

        public ActionResult ShowAllCategory()
        {

            using (var dbCT = new EF.CMSContext())
            {
                var _category = dbCT.Category.ToList();
                return View(_category);
            }
        }
        [Authorize(Roles = SecurityRoles.Admin)]
        [HttpGet]
        public ActionResult DeleteIdea(int id)
        {
            using (var ct = new CMSContext())
            {
                var idea = ct.Idea.FirstOrDefault(b => b.Id == id);

                if (idea != null)
                {
                    ct.Idea.Remove(idea);
                    ct.SaveChanges();
                    TempData["message"] = $"Successfully delete idea with name: {idea.Title}";
                }


                return RedirectToAction("ViewAllIdea");
            }
        }
    }
}
