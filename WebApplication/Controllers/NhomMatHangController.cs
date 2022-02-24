using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class NhomMatHangController : Controller
    {
        DatabaseEntities db = new DatabaseEntities();
        public ActionResult Index()
        {
            List<DNHOMMATHANG> lst = db.DNHOMMATHANGs.OrderBy(x => x.CODE).ToList();
            return View(lst);
        }

        [HttpGet]
        public ActionResult Item(string id)
        {
            DNHOMMATHANG model = db.DNHOMMATHANGs.Where(x => x.ID == id).FirstOrDefault();
            if (model == null)
            {
                model = new DNHOMMATHANG();
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddOrEdit(string id, DMATHANG temp)
        {
            string error = "";
            try
            {
                DNHOMMATHANG model = db.DNHOMMATHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (model == null)
                {
                    model = new DNHOMMATHANG();
                    model.ID = Guid.NewGuid().ToString();
                }
                model.CODE = temp.CODE.Trim();
                model.NAME = temp.NAME.Trim();
                if (id == null) db.DNHOMMATHANGs.Add(model);
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                //kiểm tra trống
                if (temp.CODE == null || temp.CODE.Length == 0) error = "Mã nhóm hàng không được trống!";
                else if (temp.NAME == null || temp.NAME.Length == 0) error = "Tên nhóm hàng không được trống!";
                //kiểm tra không được chứa mã nhóm khác
                bool contains = db.DNHOMMATHANGs.Where(x => x.CODE.Contains(temp.CODE) || temp.CODE.Contains(x.CODE)).ToList().Count > 0;
                if (contains)
                {
                    error = "Mã nhóm mặt hàng không được chứa nhóm mặt hàng khác";
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
                DNHOMMATHANG entry = db.DNHOMMATHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (entry == null) error = "Nhóm hàng không tồn tại trong hệ thống!";
                else
                {
                    db.DNHOMMATHANGs.Remove(entry);
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