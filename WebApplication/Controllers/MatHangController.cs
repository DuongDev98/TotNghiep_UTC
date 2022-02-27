using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.Controllers
{
    public class MatHangController : Controller
    {
        const string folder = "/Images/Upload/DMATHANG/";
        DatabaseEntities db = new DatabaseEntities();
        public ActionResult Index()
        {
            ViewBag.nhomHangs = db.DNHOMMATHANGs.OrderBy(x => x.CODE).ToList();
            return View();
        }

        public ActionResult Table(int page, string nhomId, string s, bool itemCart)
        {
            List<DMATHANG> lst = layDanhSachMatHang(page, nhomId, s);
            ViewBag.currentPage = page;
            ViewBag.itemCart = itemCart;
            ViewBag.nhomId = nhomId;
            ViewBag.s = s;
            return PartialView(lst);
        }

        private List<DMATHANG> layDanhSachMatHang(int page, string nhomId, string s)
        {
            //lấy danh sách
            IOrderedQueryable dataSet = db.DMATHANGs.Where(x =>
                ((s != null && s.Length > 0 && (x.CODE.Contains(s) || x.NAME.Contains(s) || x.GIANHAP.ToString().Contains(s) || x.GIABAN.ToString().Contains(s))) || s == null || s.Length == 0)
                && (nhomId == "" || nhomId == "TatCa" || x.DNHOMMATHANGID == nhomId)
            ).OrderBy(x => x.CODE);
            //tính toán phân trang
            IQueryable<DMATHANG> temp = dataSet as IQueryable<DMATHANG>;
            ViewBag.totalItem = temp.ToList().Count;
            ViewBag.numberOfPage = Math.Ceiling((double)ViewBag.totalItem / Contant.pageSize);
            return temp.Skip((page - 1) * Contant.pageSize).Take(Contant.pageSize).ToList();
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
                    DNHOMMATHANG nhomHang = db.DNHOMMATHANGs.Where(x => x.ID == temp.DNHOMMATHANGID).FirstOrDefault();
                    model = new DMATHANG();
                    model.ID = Guid.NewGuid().ToString();
                    model.CODE = GenCode(nhomHang);
                }
                model.NAME = temp.NAME.Trim();
                model.GIABAN = temp.GIABAN;
                model.GIANHAP = temp.GIANHAP;
                model.DNHOMMATHANGID = temp.DNHOMMATHANGID.Trim();
                if (id == null) db.DMATHANGs.Add(model);
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                //kiểm tra trống
                if (temp.NAME == null || temp.NAME.Length == 0) error = "Tên mặt hàng không được trống!";
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

        [HttpGet]
        public ActionResult GetImage(string id)
        {
            List<DANHSANPHAM> lstAnh = db.DANHSANPHAMs.Where(x => x.DMATHANGID == id).OrderBy(x=>x.STT).ToList();
            List<JObject> lst = new List<JObject>();
            foreach (DANHSANPHAM item in lstAnh)
            {
                JObject itNew = new JObject();
                itNew["ID"] = item.ID;
                itNew["LINK"] = folder + item.LINK;
                lst.Add(itNew);
            }
            return Content(JsonConvert.SerializeObject(lst));
        }

        public ActionResult UploadAnh(string id)
        {
            List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
            files.AddRange(Request.Files.GetMultiple("images[]"));
            uploadAnhMatHang(Request.Params.GetValues("preloaded[]"), files, id);
            //
            ViewBag.totalItem = db.DMATHANGs.ToList().Count;
            ViewBag.nhomHangs = db.DNHOMMATHANGs.OrderBy(x => x.CODE).ToList();
            return View("Index");
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
                    //xóa ảnh
                    List<DANHSANPHAM> lstRemove = db.DANHSANPHAMs.ToList();
                    foreach (DANHSANPHAM item in lstRemove)
                    {
                        FileUtils.Delete(Server, "DMATHANG", item.LINK);
                        db.DANHSANPHAMs.Remove(item);
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
    }
}