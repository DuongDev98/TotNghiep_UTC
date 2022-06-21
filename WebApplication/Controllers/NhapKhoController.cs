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
    public class NhapKhoController : Controller
    {
        DOANEntities db = new DOANEntities();
        // GET: NhapKho
        public ActionResult Index(int page, string s, string fDate, string tDate)
        {
            DateTime fromDate = (fDate != null && fDate.Length > 0 ? Convert.ToDateTime(fDate) : DateTime.Now).Date;
            DateTime toDate = (tDate != null && tDate.Length > 0 ? Convert.ToDateTime(tDate) : DateTime.Now).Date;
            //lấy danh sách
            IOrderedQueryable dataSet = db.TDONHANGs.Where(x =>
                ((s != null && s.Length > 0 && x.NAME.Contains(s)) || s == null || s.Length == 0)
                && (x.NGAY >= fromDate && x.NGAY <= toDate)
                && x.LOAI == 1
            ).OrderBy(x => x.NAME);

            //IOrderedQueryable dataSet = db.TDONHANGs.Where(x =>x.LOAI == 1).OrderBy(x => x.NAME);

            //tính toán phân trang
            IQueryable<TDONHANG> temp = dataSet as IQueryable<TDONHANG>;
            ViewBag.totalItem = temp.ToList().Count;
            ViewBag.numberOfPage = Math.Ceiling((double)ViewBag.totalItem / Contant.pageSize);
            ViewBag.currentPage = page;
            ViewBag.s = s;
            ViewBag.fDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.tDate = toDate.ToString("yyyy-MM-dd");
            List<TDONHANG> lst = temp.Skip((page - 1) * Contant.pageSize).Take(Contant.pageSize).ToList();
            return View(lst);
        }

        public ActionResult Delete(string id)
        {
            string error = "";
            try
            {
                db.TDONHANGCHITIETs.RemoveRange(db.TDONHANGCHITIETs.Where(x => x.TDONHANGID == id).ToList());
                TDONHANG entry = db.TDONHANGs.Where(x => x.ID == id).FirstOrDefault();
                db.TDONHANGs.Remove(entry);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return Content(error);
        }

        [HttpGet]
        public ActionResult AddOrUpdate(string id)
        {
            TDONHANG dhRow = db.TDONHANGs.Where(x => x.ID == id).FirstOrDefault();
            string error = "";
            try
            {
                if (dhRow == null)
                {
                    dhRow = new TDONHANG();
                    dhRow.NGAY = DateTime.Now.Date;
                    dhRow.NAME = "Tự động";
                    dhRow.TIENHANG = 0;
                    dhRow.TILEGIAMGIA = 0;
                    dhRow.TIENGIAMGIA = 0;
                    dhRow.TONGCONG = 0;
                }
                ViewBag.NGAY = ConvertTo.dateToString(dhRow.NGAY);
                ViewBag.nhaCcs = db.DNHACUNGCAPs.OrderBy(x => x.NAME).ToList();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return View(dhRow);
        }

        [HttpPost]
        public ActionResult AddOrUpdate(TDONHANG temp)
        {
            ViewBag.nhaCcs = db.DNHACUNGCAPs.OrderBy(x => x.NAME).ToList();
            if (ModelState.IsValid)
            {
                string error = "";
                try
                {
                    string ID = "";
                    TDONHANG dhRow;
                    if (temp.ID == null)
                    {
                        ID = Guid.NewGuid().ToString();
                        dhRow = new TDONHANG();
                        dhRow.NGAY = DateTime.Now;
                        dhRow.NAME = GenCodeHD("NK", 1);
                        dhRow.LOAI = 1;
                    }
                    else
                    {
                        ID = temp.ID;
                        dhRow = db.TDONHANGs.Find(temp.ID);
                        //xóa hết chi tiết thêm lại từ đầu
                        db.TDONHANGCHITIETs.RemoveRange(dhRow.TDONHANGCHITIETs);
                    }
                    decimal tienHang = 0;
                    //them chi tiet
                    foreach (TDONHANGCHITIET ctRow in temp.TDONHANGCHITIETs)
                    {
                        ctRow.THANHTIEN = ConvertTo.Decimal(ctRow.DONGIA) * ConvertTo.Decimal(ctRow.SOLUONG);
                        tienHang += ConvertTo.Decimal(ctRow.THANHTIEN);
                    }

                    dhRow.TILEGIAMGIA = temp.TILEGIAMGIA;
                    dhRow.TIENHANG = tienHang;
                    dhRow.TIENGIAMGIA = temp.TILEGIAMGIA == 0 ? 0 : ConvertTo.Decimal(dhRow.TIENHANG) * ConvertTo.Decimal(dhRow.TILEGIAMGIA) / 100;
                    dhRow.TONGCONG = dhRow.TIENHANG - ConvertTo.Decimal(dhRow.TIENGIAMGIA);
                    dhRow.DNHACUNGCAPID = temp.DNHACUNGCAPID;

                    if (temp.ID == null)
                    {
                        dhRow.ID = ID;
                        db.TDONHANGs.Add(dhRow);
                    }
                    else
                    {
                        db.Entry(dhRow).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    foreach (TDONHANGCHITIET tempRow in temp.TDONHANGCHITIETs)
                    {
                        TDONHANGCHITIET ctRow = new TDONHANGCHITIET();
                        ctRow.ID = Guid.NewGuid().ToString();
                        ctRow.TDONHANGID = ID;
                        ctRow.DMATHANGID = tempRow.DMATHANGID;
                        ctRow.DONGIA = tempRow.DONGIA;
                        ctRow.SOLUONG = tempRow.SOLUONG;
                        ctRow.THANHTIEN = tempRow.THANHTIEN;
                        ctRow.IMEI = tempRow.IMEI;
                        db.TDONHANGCHITIETs.Add(ctRow);
                    }
                    db.SaveChanges();

                    //new { page = 1, s = "", fDate = "", tDate = "" }
                    return RedirectToAction("Index", new { page = 1, s = "", fDate = "", tDate = "" });

                    //luu thanh tien
                    ViewBag.NGAY = ConvertTo.dateToString(dhRow.NGAY);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                //if (error.Length > 0) return View();
            }
            return RedirectToAction("Index", new { page = 1, s = "", fDate = "", tDate = "" });
        }

        string GenCodeHD(string startStr, int loai)
        {
            string code = "", temp = "";
            //lấy số thứ tự
            List<string> lst = db.Database.SqlQuery<string>("SELECT NAME FROM TDONHANG WHERE NAME LIKE '" + startStr + "%'").ToList();
            int max = 0;
            foreach (string item in lst)
            {
                int value = 0;
                temp = item.Replace(startStr, "");
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
            return startStr + code;
        }
    }
}