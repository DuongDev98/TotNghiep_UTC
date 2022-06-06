using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class TrangChuController : Controller
    {
        public ActionResult Index()
        {
            //ApiController.exportDuLieuDiaDiem(new Models.DOANEntities());
            return View();
        }
    }
}