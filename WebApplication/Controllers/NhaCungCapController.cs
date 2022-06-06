using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class NhaCungCapController : Controller
    {
        DOANEntities db = new DOANEntities();
        public ActionResult Index()
        {
            List<DNHACUNGCAP> lst = db.DNHACUNGCAPs.OrderBy(x => x.NAME).ToList();
            return View(lst);
        }

        [HttpGet]
        public ActionResult Item(string id)
        {
            DNHACUNGCAP model = db.DNHACUNGCAPs.Where(x => x.ID == id).FirstOrDefault();
            if (model == null)
            {
                model = new DNHACUNGCAP();
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddOrEdit(string id, DNHACUNGCAP temp)
        {
            string error = "";
            try
            {
                //kiểm tra trống
                if (temp.NAME == null || temp.NAME.Length == 0) error = "Tên nhà cung cấp không được trống!";
                else if (temp.DIENTHOAI == null || temp.DIENTHOAI.Length == 0) error = "Điện thoại không được trống!";
                else if (temp.DIACHI == null || temp.DIACHI.Length == 0) error = "Địa chỉ không được trống!";
                //không được trùng tên
                bool contains = db.DNHACUNGCAPs.Where(x => (x.NAME == temp.NAME) && x.ID != id).ToList().Count > 0;
                if (contains)
                {
                    error = "Tên nhà cung cấp đã tồn tại";
                }

                if (error.Length == 0)
                {
                    DNHACUNGCAP model = db.DNHACUNGCAPs.Where(x => x.ID == id).FirstOrDefault();
                    if (model == null)
                    {
                        model = new DNHACUNGCAP();
                    }
                    model.NAME = temp.NAME == null ? "" : temp.NAME.Trim();
                    model.DIENTHOAI = temp.DIENTHOAI == null ? "" : temp.DIENTHOAI.Trim();
                    model.DIACHI = temp.DIACHI == null ? "" : temp.DIACHI.Trim();
                    model.EMAIL = temp.EMAIL == null ? "" : temp.EMAIL.Trim();
                    if (id == null)
                    {
                        model.ID = Guid.NewGuid().ToString();
                        db.DNHACUNGCAPs.Add(model);
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
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
                DNHACUNGCAP entry = db.DNHACUNGCAPs.Where(x => x.ID == id).FirstOrDefault();
                if (entry == null) error = "Nhà cung cấp không tồn tại trong hệ thống!";
                else
                {
                    db.DNHACUNGCAPs.Remove(entry);
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