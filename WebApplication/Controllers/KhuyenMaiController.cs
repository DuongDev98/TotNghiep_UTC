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
    public class KhuyenMaiController : Controller
    {
        private DOANEntities db = new DOANEntities();
        public ActionResult Index()
        {
            return View(db.DKHUYENMAIs.ToList());
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                id = "";
            }
            DKHUYENMAI dKHUYENMAI = db.DKHUYENMAIs.Find(id);
            if (dKHUYENMAI == null)
            {
                dKHUYENMAI = new DKHUYENMAI();
            }
            return View(dKHUYENMAI);
        }

        [HttpPost]
        public ActionResult Edit(DKHUYENMAI temp)
        {
            DKHUYENMAI dKHUYENMAI = null;
            if (ModelState.IsValid)
            {
                if (temp.ID == null)
                {
                    dKHUYENMAI = new DKHUYENMAI();
                }
                else
                {
                    dKHUYENMAI = db.DKHUYENMAIs.Find(temp.ID);
                }
                dKHUYENMAI.NAME = temp.NAME;
                dKHUYENMAI.TUGIO = temp.TUGIO;
                dKHUYENMAI.DENGIO = temp.DENGIO;
                dKHUYENMAI.TUNGAY = temp.TUNGAY;
                dKHUYENMAI.DENNGAY = temp.DENNGAY;
                dKHUYENMAI.PHANTRAMGIAMGIA = temp.PHANTRAMGIAMGIA;
                dKHUYENMAI.TONGBILL = temp.TONGBILL;

                if (temp.ID == null)
                {
                    dKHUYENMAI.ID = Guid.NewGuid().ToString();
                    db.DKHUYENMAIs.Add(dKHUYENMAI);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(dKHUYENMAI).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(dKHUYENMAI);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DKHUYENMAI dKHUYENMAI = db.DKHUYENMAIs.Find(id);
            if (dKHUYENMAI == null)
            {
                return HttpNotFound();
            }
            return View(dKHUYENMAI);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DKHUYENMAI dKHUYENMAI = db.DKHUYENMAIs.Find(id);
            db.DKHUYENMAIs.Remove(dKHUYENMAI);
            db.SaveChanges();
            return RedirectToAction("Index");
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
