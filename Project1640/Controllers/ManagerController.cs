using ICSharpCode.SharpZipLib.Zip;
using Project1640.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public ActionResult EditCategory(int id)
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

        

    }

}
