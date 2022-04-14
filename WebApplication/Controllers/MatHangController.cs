using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.Controllers
{
    public class MatHangController : Controller
    {
        private DOANEntities db = new DOANEntities();

        // GET: MatHang
        public ActionResult Index()
        {
            var dMATHANGs = db.DMATHANGs.Include(d => d.DNHOMMATHANG).Include(d => d.DTHUONGHIEU);
            return View(dMATHANGs.ToList());
        }

        // GET: MatHang/Edit/5
        public ActionResult Edit(string id)
        {
            DMATHANG dMATHANG = db.DMATHANGs.Find(id);
            if (dMATHANG == null)
            {
                dMATHANG = new DMATHANG();
                dMATHANG.CODE = "Tự động";
            }
            ViewBag.imgs = GetDicAnhs(db, dMATHANG);
            ViewBag.DNHOMMATHANGID = new SelectList(db.DNHOMMATHANGs, "ID", "CODE", dMATHANG.DNHOMMATHANGID);
            ViewBag.DTHUONGHIEUID = new SelectList(db.DTHUONGHIEUs, "ID", "NAME", dMATHANG.DTHUONGHIEUID);
            return View(dMATHANG);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CODE,NAME,GIANHAP,GIABAN,DNHOMMATHANGID,DTHUONGHIEUID,MOTA,COIMEI")] DMATHANG temp)
        {
            string error = "";
            DMATHANG model = db.DMATHANGs.Where(x => x.ID == temp.ID).FirstOrDefault();
            try
            {
                if (model == null)
                {
                    model = new DMATHANG();
                }
                model.NAME = temp.NAME;
                model.GIABAN = temp.GIABAN;
                model.GIANHAP = temp.GIANHAP;
                model.DNHOMMATHANGID = temp.DNHOMMATHANGID.Trim();
                model.DTHUONGHIEUID = temp.DTHUONGHIEUID.Trim();
                model.MOTA = temp.MOTA;
                model.COIMEI = temp.COIMEI == null ? 0 : temp.COIMEI;
                //kiểm tra trống
                if (temp.NAME == null || temp.NAME.Length == 0) error = "Tên mặt hàng không được trống!";
                //không được trùng tên
                bool contains = db.DMATHANGs.Where(x => x.NAME == temp.NAME && x.ID != temp.ID && temp.ID != null).ToList().Count > 0;
                if (contains)
                {
                    error = "Tên mặt hàng đã tồn tại đã tồn tại";
                }

                if (error.Length == 0)
                {
                    if (temp.ID == null)
                    {
                        DNHOMMATHANG nhomHang = db.DNHOMMATHANGs.Where(x => x.ID == temp.DNHOMMATHANGID).FirstOrDefault();
                        model.ID = Guid.NewGuid().ToString();
                        model.CODE = GenCode(nhomHang);
                        db.DMATHANGs.Add(model);
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    //Lưu ảnh
                    List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
                    files.AddRange(Request.Files.GetMultiple("images[]"));
                    uploadAnhMatHang(Request.Params.GetValues("preloaded[]"), files, model.ID);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error.Length > 0) ViewBag.error = error;
            ViewBag.DNHOMMATHANGID = new SelectList(db.DNHOMMATHANGs, "ID", "CODE", model.DNHOMMATHANGID);
            ViewBag.DTHUONGHIEUID = new SelectList(db.DTHUONGHIEUs, "ID", "NAME", model.DTHUONGHIEUID);
            return View(model);
        }

        public static Dictionary<string, string> GetDicAnhs(DOANEntities db, DMATHANG mhRow)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            List<DANHSANPHAM> lstAnhs = db.DANHSANPHAMs.Where(x => x.DMATHANGID == mhRow.ID).ToList();
            foreach (DANHSANPHAM anh in lstAnhs)
            {
                dic.Add(anh.ID, "/Images/Upload/DMATHANG/" + anh.LINK);
            }
            return dic;
        }

        private void uploadAnhMatHang(string[] olds, List<HttpPostedFileBase> files, string DMATHANGID)
        {
            //xóa hết ảnh cũ nếu không còn, thêm ảnh mới và đánh số thứ tự
            int i = 1;
            List<DANHSANPHAM> lstRemove = db.DANHSANPHAMs.ToList();
            foreach (DANHSANPHAM item in lstRemove)
            {
                bool delete = false;
                if (olds == null) delete = true;
                if (!delete)
                {
                    delete = true;
                    foreach (string idOld in olds)
                    {
                        if (item.ID == idOld)
                        {
                            delete = false;
                        }
                    }
                }
                if (delete)
                {
                    db.DANHSANPHAMs.Remove(item);
                    FileUtils.Delete(Server, "DMATHANG", item.LINK);
                }
                else
                {
                    item.STT = i;
                    i++;
                }
            }
            //Lưu ảnh mới
            foreach (HttpPostedFileBase file in files)
            {
                if (file.ContentLength == 0) continue;
                string link = FileUtils.Upload(Server, "DMATHANG", file);
                DANHSANPHAM anh = new DANHSANPHAM();
                anh.LINK = link;
                anh.DMATHANGID = DMATHANGID;
                anh.ID = Guid.NewGuid().ToString();
                db.DANHSANPHAMs.Add(anh);
                i++;
            }
            db.SaveChanges();
        }

        string GenCode(DNHOMMATHANG nhomHang)
        {
            string code = nhomHang.CODE, temp = "";
            //lấy số thứ tự
            List<string> lst = db.Database.SqlQuery<string>("SELECT CODE FROM DMATHANG WHERE CODE LIKE '" + nhomHang.CODE + "%'").ToList();
            int max = 0;
            foreach (string item in lst)
            {
                int value = 0;
                temp = item.Replace(nhomHang.CODE, "");
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

        // GET: MatHang/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DMATHANG dMATHANG = db.DMATHANGs.Find(id);
            if (dMATHANG == null)
            {
                return HttpNotFound();
            }
            if (dMATHANG.TDONHANGCHITIETs.Count > 0) ViewBag.error = "Mặt hàng đã phát sinh đơn hàng, không thể xóa";
            return View(dMATHANG);
        }

        // POST: MatHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DMATHANG dMATHANG = db.DMATHANGs.Find(id);
            db.DMATHANGs.Remove(dMATHANG);
            //xóa ảnh
            List<DANHSANPHAM> lstRemove = db.DANHSANPHAMs.ToList();
            foreach (DANHSANPHAM item in lstRemove)
            {
                FileUtils.Delete(Server, "DMATHANG", item.LINK);
                db.DANHSANPHAMs.Remove(item);
            }
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

        public ActionResult Table(int page, string nhomId, string thuongHieuId, string s, bool itemCart)
        {
            List<DMATHANG> lst = layDanhSachMatHang(page, nhomId, thuongHieuId, s);
            ViewBag.currentPage = page;
            ViewBag.itemCart = itemCart;
            ViewBag.nhomId = nhomId;
            ViewBag.thuongHieuId = thuongHieuId;
            ViewBag.s = s;
            return PartialView(lst);
        }

        private List<DMATHANG> layDanhSachMatHang(int page, string nhomId, string thuongHieuId, string s)
        {
            //lấy danh sách
            IOrderedQueryable dataSet = db.DMATHANGs.Where(x =>
                ((s != null && s.Length > 0 && (x.CODE.Contains(s) || x.NAME.Contains(s) || x.GIANHAP.ToString().Contains(s) || x.GIABAN.ToString().Contains(s))) || s == null || s.Length == 0)
                && (nhomId == "" || nhomId == "TatCa" || x.DNHOMMATHANGID == nhomId)
                && (thuongHieuId == "" || thuongHieuId == "TatCa" || x.DTHUONGHIEUID == thuongHieuId)
            ).OrderBy(x => x.CODE);
            //tính toán phân trang
            IQueryable<DMATHANG> temp = dataSet as IQueryable<DMATHANG>;
            ViewBag.totalItem = temp.ToList().Count;
            ViewBag.numberOfPage = Math.Ceiling((double)ViewBag.totalItem / Contant.pageSize);
            return temp.Skip((page - 1) * Contant.pageSize).Take(Contant.pageSize).ToList();
        }
    }
}
