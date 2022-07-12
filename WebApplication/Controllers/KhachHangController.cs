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
    public class KhachHangController : Controller
    {
        private DOANEntities db = new DOANEntities();
        public ActionResult Index(int? page)
        {
            var dKHACHHANGs = db.DKHACHHANGs.Include(d => d.DPHUONGXA).Include(d => d.DQUANHUYEN)
                .Include(d => d.DTINHTHANH).OrderBy(x=>x.NAME);
            int pageSize = 10;
            int pageNumber = page ?? 1;
            return View(dKHACHHANGs.ToPagedList(pageNumber, pageSize));
        }
    }
}