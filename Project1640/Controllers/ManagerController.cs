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
            using (var cate = new EF.CMSContext())
            {
                cate.Entry<Category>(a).State = System.Data.Entity.EntityState.Modified;

                cate.SaveChanges();
            }

            return RedirectToAction("Index");
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
                TempData["message"] = $"Successfully delete book with Id: {Category.Id}";
                return RedirectToAction("Index");
            }
        }
        //-----

    }
}