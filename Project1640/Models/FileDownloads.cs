using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project1640;
using System.IO;

namespace Project1640.Models
{
    public class FileDownloads
    {
        public List<FileInfo> GetFile()
        {

            List<FileInfo> listFiles = new List<FileInfo>();

            string fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~/FileUpload");

            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);

            int i = 0;

            foreach (var item in dirInfo.GetFiles())
            {
                listFiles.Add(new FileInfo()
                {

                    FileId = i + 1,

                    FileName = item.Name,

                    FilePath = dirInfo.FullName + @"\" + item.Name

                });

                i = i + 1;

            }

            return listFiles;

        }

    }
}