using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication.Utils
{
    public class FileUtils
    {
        public static string Upload(HttpServerUtilityBase server, string folder, HttpPostedFileBase file)
        {
            string tempFolder = Path.Combine(server.MapPath("~/Images/Upload/" + folder));
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            string temp = file.FileName;
            string[] options = temp.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            string fileName = Guid.NewGuid().ToString();
            if (options.Length > 1)
            {
                fileName = fileName + "." + options[options.Length - 1];
            }
            else
            {
                fileName = fileName + ".jpg";
            }

            string path = Path.Combine(server.MapPath("~/Images/Upload/" + folder + "/" + fileName));

            if (File.Exists(path)) File.Delete(path);

            file.SaveAs(path);

            return fileName;
        }

        public static void Delete(HttpServerUtilityBase server, string folder, string fileName)
        {
            string path = Path.Combine(server.MapPath("~/Images/Upload/" + folder + "/" + fileName));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}