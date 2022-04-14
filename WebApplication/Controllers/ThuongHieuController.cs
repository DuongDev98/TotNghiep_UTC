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
    public class ThuongHieuController : Controller
    {
        DOANEntities db = new DOANEntities();
        public ActionResult Index()
        {
            List<DTHUONGHIEU> lst = db.DTHUONGHIEUs.OrderBy(x => x.NAME).ToList();
            return View(lst);
        }

        [HttpGet]
        public ActionResult Item(string id)
        {
            DTHUONGHIEU model = db.DTHUONGHIEUs.Where(x => x.ID == id).FirstOrDefault();
            if (model == null)
            {
                model = new DTHUONGHIEU();
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddOrEdit(string id, DTHUONGHIEU temp)
        {
            string error = "";
            try
            {
                //kiểm tra trống
                if (temp.NAME == null || temp.NAME.Length == 0) error = "Tên nhóm hàng không được trống!";

                if (error.Length == 0)
                {
                    DTHUONGHIEU model = db.DTHUONGHIEUs.Where(x => x.ID == id).FirstOrDefault();
                    if (model == null)
                    {
                        model = new DTHUONGHIEU();
                    }
                    model.NAME = temp.NAME.Trim();
                    if (id == null)
                    {
                        model.ID = Guid.NewGuid().ToString();
                        db.DTHUONGHIEUs.Add(model);
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
                DTHUONGHIEU entry = db.DTHUONGHIEUs.Where(x => x.ID == id).FirstOrDefault();
                if (entry == null) error = "Thương hiệu không tồn tại trong hệ thống!";
                else
                {
                    if (entry.DMATHANGs.Count > 0)
                    {
                        error = "có " + entry.DMATHANGs.Count + " liên kết tới thương hiệu, không thể xóa";
                    }
                    else
                    {
                        db.DTHUONGHIEUs.Remove(entry);
                        db.SaveChanges();
                    }
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