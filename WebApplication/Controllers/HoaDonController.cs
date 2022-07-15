using PagedList;
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
        public ActionResult Index(int? page)
        {
            var tDONHANGs = db.TDONHANGs.Where(t=>t.LOAI == 0).Include(t => t.DKHACHHANG).Include(t => t.DNHACUNGCAP).Include(t => t.DPHUONGXA).Include(t => t.DQUANHUYEN).Include(t => t.DTINHTHANH).
                OrderByDescending(t=>t.NGAY).OrderByDescending(t => t.NAME);
            foreach (var t in tDONHANGs)
            {
                t.TIENHANG = t.TIENHANG ?? 0;
                t.TILEGIAMGIA = t.TILEGIAMGIA ?? 0;
                t.TIENGIAMGIA = t.TIENGIAMGIA ?? 0;
                t.TONGCONG = t.TONGCONG ?? 0;
            }
            int pageSize = 10;
            int pageNumber = page ?? 1;
            return View(tDONHANGs.ToPagedList(pageNumber, pageSize));
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
            List<string> lst = db.Database.SqlQuery<string>("SELECT NAME FROM TDONHANG WHERE NAME LIKE '" + start + "%' ORDER BY NAME DESC").Take(1).ToList();
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
            string error = "";
            try
            {
                if (imei == null || imei.Length == 0)
                {
                    error = "Imei không được trống";
                }
                else
                {
                    TDONHANGCHITIET ctRow = db.TDONHANGCHITIETs.Find(id);
                    var rs = db.TDONHANGCHITIETs.Where(x => x.TDONHANG.LOAI == 0 && ctRow.DMATHANGID == x.DMATHANGID && x.IMEI == imei).ToList();
                    if (rs.Count == 0)
                    {
                        //rs = db.TDONHANGCHITIETs.Where(x => x.TDONHANG.LOAI == 1 && ctRow.DMATHANGID == x.DMATHANGID && x.IMEI == imei).ToList();
                        //if (rs.Count == 0)
                        //{
                        //    error = "Sản phẩm có imei không tồn tại trong kho";
                        //}
                        //else
                        //{
                        //}

                        ctRow.IMEI = imei;
                        db.Entry(ctRow);
                        db.SaveChanges();
                    }
                    else
                    {
                        error = "Sản phẩm có imei đã được bán trước đó";
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return Content(error);
        }
        public ActionResult CapNhatTrangThai(string id, string trangThai)
        {
            try
            {
                TDONHANG dhRow = db.TDONHANGs.Find(id);
                dhRow.TRANGTHAI = int.Parse(trangThai);
                db.Entry(dhRow);
                db.SaveChanges();
                //gui email
                string noiDung = ThuChiController.RenderRazorViewToString(ControllerContext, ViewData, TempData, "ViewPdf", dhRow);
                ApiController.SendEmail("Thay đổi trạng thái đơn hàng", dhRow.DKHACHHANG.EMAIL, noiDung);
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

        public ActionResult ExportPdf(string id)
        {
            TDONHANG dhRow = db.TDONHANGs.Find(id);
            byte[] dataArr = ThuChiController.GetFilePDF(ControllerContext, ViewData, TempData, "ViewPdf", dhRow);
            return File(dataArr, "application/pdf", dhRow.NAME + ".pdf");
            //return View("ViewPdf", dhRow);
        }
    }
}
