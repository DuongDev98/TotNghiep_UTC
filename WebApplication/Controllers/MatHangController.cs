using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class MatHangController : Controller
    {
        const int maxPage = 20;
        DatabaseEntities db = new DatabaseEntities();
        public ActionResult Index()
        {
            ViewBag.totalItem = db.DMATHANGs.ToList().Count;
            ViewBag.nhomHangs = db.DNHOMMATHANGs.OrderBy(x => x.CODE).ToList();
            return View();
        }

        public ActionResult TBodyTable(int page, string nhomId, string s)
        {
            List<DMATHANG> lst = layDanhSachMatHang(page, nhomId, s);
            ViewBag.currentPage = page;
            return PartialView(lst);
        }

        private List<DMATHANG> layDanhSachMatHang(int page, string nhomId, string s)
        {
            return db.DMATHANGs.Where(x =>
                ((s != null && s.Length > 0 && (x.CODE.Contains(s) || x.NAME.Contains(s) || x.GIANHAP.ToString().Contains(s) || x.GIABAN.ToString().Contains(s))) || s == null || s.Length == 0)
                && (nhomId == "" || nhomId == "TatCa" || x.DNHOMMATHANGID == nhomId)
            ).OrderBy(x=>x.CODE).Skip((page - 1) * maxPage).Take(maxPage).ToList();
        }

        [HttpGet]
        public ActionResult Item(string id)
        {
            DMATHANG model = db.DMATHANGs.Where(x => x.ID == id).FirstOrDefault();
            if (model == null)
            {
                model = new DMATHANG();
            }
            ViewBag.nhomHangs = db.DNHOMMATHANGs.OrderBy(x=>x.CODE).ToList();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddOrEdit(string id, DMATHANG temp)
        {
            string error = "";
            try
            {
                DMATHANG model = db.DMATHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DMATHANG();
                    model.ID = Guid.NewGuid().ToString();
                }
                model.NAME = temp.NAME.Trim();
                //model.DIENTHOAI = temp.DIENTHOAI.Trim();
                //model.DIACHI = temp.DIACHI.Trim();
                //model.EMAIL = temp.EMAIL.Trim();
                if (id == null) db.DMATHANGs.Add(model);
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                //kiểm tra trống
                if (temp.NAME == null || temp.NAME.Length == 0) error = "Tên nhà cung cấp không được trống!";
                //else if (temp.DIENTHOAI == null || temp.DIENTHOAI.Length == 0) error = "Điện thoại không được trống!";
                //else if (temp.DIACHI == null || temp.DIACHI.Length == 0) error = "Địa chỉ không được trống!";
                //không được trùng tên
                bool contains = db.DMATHANGs.Where(x => x.NAME == temp.NAME).ToList().Count > 0;
                if (contains)
                {
                    error = "Tên mặt hàng đã tồn tại đã tồn tại";
                }

                if (error.Length == 0) db.SaveChanges();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return Content(error);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            string error = "";
            try
            {
                DMATHANG entry = db.DMATHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (entry == null) error = "Mặt hàng không tồn tại trong hệ thống!";
                else
                {
                    db.DMATHANGs.Remove(entry);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return Content(error);
        }
    }
}