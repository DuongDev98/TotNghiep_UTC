using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Utils;
using WebApplication.Models;

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
    }
}