using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Utils;
using WebApplication.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml;

namespace WebApplication.Controllers
{
    public class ThuChiController : Controller
    {
        DOANEntities db = new DOANEntities();
        // GET: ThuChi
        public ActionResult Index()
        {
            string query = @"SELECT ID, NAME, FORMAT(NGAY, 'dd/MM/yyyy') AS NGAY, (SELECT NAME FROM DNHACUNGCAP WHERE DNHACUNGCAPID = DNHACUNGCAP.ID) AS NHACUNGCAP, CHI FROM TTHUCHI ORDER BY NAME DESC";
            DataTable dt = DatabaseUtils.GetTable(query);
            return View(dt);
        }

        [HttpGet]
        public ActionResult AddOrEdit(string id, string DNHACUNGCAPID, int? soTien)
        {
            TTHUCHI kq = null;
            if (id == null || id.Length == 0)
            {
                kq = new TTHUCHI();
                kq.NAME = "Tự động";
                kq.DNHACUNGCAPID = DNHACUNGCAPID;
                kq.NGAY = DateTime.Now;
                kq.CHI = soTien;
            }
            else
            {
                kq = db.TTHUCHIs.Find(id);
            }
            ViewBag.NGAY = ConvertTo.dateToString(kq.NGAY);
            ViewBag.nhaCcs = db.DNHACUNGCAPs.ToList();
            return View(kq);
        }

        [HttpPost]
        public ActionResult AddOrEdit(TTHUCHI temp)
        {
            if (ModelState.IsValid)
            {
                TTHUCHI kq = null;
                if (temp.ID == null || temp.ID.Length == 0)
                {
                    kq = new TTHUCHI();
                    kq.ID = Guid.NewGuid().ToString();
                    kq.NAME = GenCode(db, "PC");
                    kq.DNHACUNGCAPID = temp.DNHACUNGCAPID;
                    kq.NGAY = DateTime.Now;
                    kq.CHI = temp.CHI;
                    kq.LOAI = 1;
                    kq.THU = 0;
                    db.TTHUCHIs.Add(kq);
                }
                else
                {
                    kq = db.TTHUCHIs.Find(temp.ID);
                    kq.CHI = temp.CHI;
                    kq.DNHACUNGCAPID = temp.DNHACUNGCAPID;
                    db.Entry(kq);
                }
                db.SaveChanges();
            }
            //ViewBag.NGAY = ConvertTo.dateToString(kq.NGAY);
            //ViewBag.nhaCcs = db.DNHACUNGCAPs.ToList();
            return RedirectToAction("CongNo", "NhaCungCap");
        }

        public ActionResult Delete(string id)
        {
            TTHUCHI kq = db.TTHUCHIs.Find(id);
            if (kq != null)
            {
                db.TTHUCHIs.Remove(kq);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public static string GenCode(DOANEntities db, string start)
        {
            string code = start, temp = "";
            //lấy số thứ tự
            List<string> lst = db.Database.SqlQuery<string>("SELECT NAME FROM TTHUCHI WHERE NAME LIKE '" + start + "%' ORDER BY NAME DESC").Take(1).ToList();
            int max = 0;
            foreach (string item in lst)
            {
                int value = 0;
                temp = item.Replace(start, "");
                value = Convert.ToInt32(temp);
                max = value > max ? value : max;
            }
            max = max + 1;
            int maxLen = 6;
            temp = "";
            while (temp.Length + max.ToString().Length < maxLen)
            {
                temp += "0";
            }
            temp += max.ToString();
            code += temp;
            return code;
        }

        public ActionResult ExportPdf(string id)
        {
            TTHUCHI tcRow = db.TTHUCHIs.Find(id);
            byte[] dataArr = GetFilePDF(ControllerContext, ViewData, TempData, "ViewPdf", tcRow);
            return File(dataArr, "application/pdf", tcRow.NAME + ".pdf");
        }

        public static byte[] GetFilePDF(ControllerContext controls, ViewDataDictionary ViewData, TempDataDictionary TempData, string viewName, object model)
        {
            string htmlContent = RenderRazorViewToString(controls, ViewData, TempData, viewName, model);
            using (MemoryStream stream = new MemoryStream())
            {
                StringReader sr = new StringReader(htmlContent);
                Document pdfDoc = new Document(PageSize.POSTCARD, 10f, 10f, 10f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                byte[] arr = System.Text.Encoding.UTF8.GetBytes(htmlContent);
                MemoryStream ms = new MemoryStream(arr);
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, ms, null, System.Text.Encoding.UTF8,
                    new UnicodeFontFactory(controls.HttpContext.Server.MapPath("~/fonts/tahoma.ttf")));
                pdfDoc.Close();
                return stream.ToArray();
            }
        }

        public static string RenderRazorViewToString(ControllerContext ControllerContext, ViewDataDictionary ViewData, TempDataDictionary TempData, string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
    public class UnicodeFontFactory : FontFactoryImp
    {
        private readonly BaseFont _baseFont;
        public UnicodeFontFactory(string FontPath)
        {
            _baseFont = BaseFont.CreateFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }
        public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached)
        {
            return new Font(_baseFont, size, style, color);
        }
    }
}