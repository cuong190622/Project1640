﻿using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Project1640.EF;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

    }

}
