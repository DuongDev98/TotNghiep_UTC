using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.Controllers
{
    public class KhoController : Controller
    {
        DOANEntities db = new DOANEntities();

        public ActionResult Index()
        {
            string query = @"SELECT CODE, NAME, GIANHAP, GIABAN,
(SELECT NAME FROM DNHOMMATHANG WHERE DNHOMMATHANG.ID = DNHOMMATHANGID) AS NHOM,
(SELECT NAME FROM DTHUONGHIEU WHERE DTHUONGHIEU.ID = DTHUONGHIEUID) AS THUONGHIEU,
(
	COALESCE((SELECT SUM(CASE WHEN TDONHANG.LOAI = 1 THEN SOLUONG ELSE -SOLUONG END) FROM TDONHANGCHITIET
	INNER JOIN TDONHANG ON TDONHANGID = TDONHANG.ID WHERE DMATHANGID = DMATHANG.ID), 0)
) AS TON
FROM DMATHANG";
            DataTable dt = DatabaseUtils.GetTable(query);
            return View(dt);
        }

        [HttpGet]
        public ActionResult TraCuu()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TraCuu(string imei)
        {
            ViewBag.imei = imei;
            List<TDONHANGCHITIET> lstTemp = db.TDONHANGCHITIETs.Where(x => x.IMEI.ToLower() == imei.ToLower()).Take(1).ToList();
            return View(lstTemp);
        }
    }
}