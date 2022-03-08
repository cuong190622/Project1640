using Project1640.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            using (var cate = new EF.CMSContext())
            {
                cate.Category.Add(a);
                cate.SaveChanges();
            }

            TempData["message"] = $"Successfully add class {a.Name} to system!";

            return RedirectToAction("Index");
        }
    }
}