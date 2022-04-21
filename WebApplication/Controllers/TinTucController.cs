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
    public class TinTucController : Controller
    {
        private DOANEntities db = new DOANEntities();

        // GET: TinTuc
        public ActionResult Index()
        {
            return View(db.DTINTUCs.ToList());
        }

        // GET: TinTuc/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: TinTuc/Edit/5
        public ActionResult Edit(string id)
        {
            DTINTUC dTINTUC = db.DTINTUCs.Find(id);
            if (dTINTUC == null) dTINTUC = new DTINTUC();
            return View(dTINTUC);
        }

        // POST: TinTuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TIMECREATED,TITLE,NOIDUNG")] DTINTUC temp)
        {
            string error = "";
            DTINTUC model = db.DTINTUCs.Where(x => x.ID == temp.ID).FirstOrDefault();
            try
            {
                if (model == null)
                {
                    model = new DTINTUC();
                }
                model.TITLE = temp.TITLE;
                model.NOIDUNG = temp.NOIDUNG;

                if (error.Length == 0)
                {
                    if (temp.ID == null)
                    {
                        model.ID = Guid.NewGuid().ToString();
                        model.TIMECREATED = DateTime.Now;
                        db.DTINTUCs.Add(model);
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error.Length > 0) ViewBag.error = error;
            return View(model);
        }

        // GET: TinTuc/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DTINTUC dTINTUC = db.DTINTUCs.Find(id);
            if (dTINTUC == null)
            {
                return HttpNotFound();
            }
            return View(dTINTUC);
        }

        // POST: TinTuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DTINTUC dTINTUC = db.DTINTUCs.Find(id);
            db.DTINTUCs.Remove(dTINTUC);
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
