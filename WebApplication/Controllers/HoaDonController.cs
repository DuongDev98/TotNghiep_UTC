using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HoaDonController : Controller
    {
        private DOANEntities db = new DOANEntities();

        public ActionResult Index()
        {
            var tDONHANGs = db.TDONHANGs.Where(t=>t.LOAI == 0).Include(t => t.DKHACHHANG).Include(t => t.DKHUYENMAI).Include(t => t.DNHACUNGCAP).Include(t => t.DPHUONGXA).Include(t => t.DQUANHUYEN).Include(t => t.DTINHTHANH);
            foreach (var t in tDONHANGs)
            {
                t.TIENHANG = t.TIENHANG ?? 0;
                t.TILEGIAMGIA = t.TILEGIAMGIA ?? 0;
                t.TIENGIAMGIA = t.TIENGIAMGIA ?? 0;
                t.TONGCONG = t.TONGCONG ?? 0;
            }
            return View(tDONHANGs.ToList());
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TDONHANG tDONHANG = db.TDONHANGs.Find(id);
            if (tDONHANG == null)
            {
                return HttpNotFound();
            }
            return View(tDONHANG);
        }

        public static string GenCode(DOANEntities db, string start)
        {
            string code = start, temp = "";
            //lấy số thứ tự
            List<string> lst = db.Database.SqlQuery<string>("SELECT CODE FROM DMATHANG WHERE CODE LIKE '" + start + "%'").ToList();
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

        public ActionResult CapNhatImei(string id, string imei)
        {
            try
            {
                TDONHANGCHITIET ctRow = db.TDONHANGCHITIETs.Find(id);
                ctRow.IMEI = imei;
                db.Entry(ctRow);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("");
        }
        public ActionResult CapNhatTrangThai(string id, string trangThai)
        {
            try
            {
                TDONHANG dhRow = db.TDONHANGs.Find(id);
                dhRow.TRANGTHAI = int.Parse(trangThai);
                db.Entry(dhRow);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
