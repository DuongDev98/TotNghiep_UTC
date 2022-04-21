using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.Controllers
{
    public class ApiController : Controller
    {
        private DOANEntities db = new DOANEntities();

        public ActionResult v1()
        {
            string error = "";
            BaseResult kq = new BaseResult();
            try
            {
                using (Stream stream = Request.InputStream)
                {
                    string json = new StreamReader(stream).ReadToEnd();
                    ParamResult attr = (ParamResult)JsonConvert.DeserializeObject(json, typeof(ParamResult));
                    if (attr.cmdtype == null || attr.cmdtype.Length == 0) error = "cmdtype không hợp lệ";
                    else
                    {
                        switch (attr.cmdtype)
                        {
                            case "ping": kq.data = null; break;
                            case "login": kq.data = login(attr.param, ref error); break;
                            case "register": kq.data = register(attr.param, ref error); break;
                            case "getUserInfo": kq.data = getUserInfo(attr.param, ref error); break;
                            case "dsThuongHieu": kq.data = layDanhSachThuongHieu(attr.param, ref error); break;
                            case "dsNhom": kq.data = layDanhSachNhom(attr.param, ref error); break;
                            case "timKiemMatHang": kq.data = timKiemMatHang(attr.param, ref error); break;
                            case "dsTinTuc": kq.data = layDanhSachTinTuc(attr.param, ref error); break;
                            case "dsTinhThanh": kq.data = layDanhSachTinhThanh(attr.param, ref error); break;
                            case "dsQuanHuyen": kq.data = layDanhSachQuanHuyen(attr.param, ref error); break;
                            case "dsPhuongXa": kq.data = layDanhSachPhuongXa(attr.param, ref error); break;
                            default: error = "cmdtype không hợp lệ"; break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            kq.isSuccess = error.Length == 0;
            kq.message = error;
            return Content(JsonConvert.SerializeObject(kq));
        }

        private JObject layDanhSachPhuongXa(JObject param, ref string error)
        {
            throw new NotImplementedException();
        }

        private JObject layDanhSachQuanHuyen(JObject param, ref string error)
        {
            throw new NotImplementedException();
        }

        private JObject layDanhSachTinhThanh(JObject param, ref string error)
        {
            List<DTINHTHANH> lst = db.DTINHTHANHs.OrderBy(x => x.NAME).ToList();
            foreach (DTINHTHANH item in lst)
            {
                item.DKHACHHANGs = null;
            }
            JArray arr = JArray.FromObject(lst);
            JObject kq = new JObject();
            kq["arr"] = arr;
            return kq;
        }

        private JObject layDanhSachTinTuc(JObject param, ref string error)
        {
            List<DTINTUC> lst = db.DTINTUCs.OrderByDescending(x => x.TIMECREATED).ToList();
            JArray arr = JArray.FromObject(lst);
            foreach (JObject it in arr)
            {
                it["TIMECREATED"] = ConvertTo.Date(it["TIMECREATED"]).ToString("HH:mm dd/MM/yyyy");
                it["NOIDUNG"] = HttpUtility.HtmlDecode(ConvertTo.String(it["NOIDUNG"]));
            }
            JObject item = new JObject();
            item["arr"] = arr;
            return item;
        }

        private JObject timKiemMatHang(JObject param, ref string error)
        {
            string s = ConvertTo.String(param["s"]);
            string DNHOMMATHANGID = ConvertTo.String(param["DNHOMMATHANGID"]);
            string DTHUONGHIEUID = ConvertTo.String(param["DTHUONGHIEUID"]);
            IQueryable<DMATHANG> lst = db.DMATHANGs.Where(x=>x.NAME.Contains(s) || s == "");
            lst = lst.Where(x => x.DNHOMMATHANGID == DNHOMMATHANGID || DNHOMMATHANGID == "");
            lst = lst.Where(x => x.DTHUONGHIEUID == DTHUONGHIEUID || DTHUONGHIEUID == "");
            return JObject.FromObject(lst.ToList());
        }

        private JObject layDanhSachNhom(JObject param, ref string error)
        {
            List<DNHOMMATHANG> lst = db.DNHOMMATHANGs.OrderBy(x => x.NAME).ToList();
            return JObject.FromObject(lst);
        }

        private JObject layDanhSachThuongHieu(JObject param, ref string error)
        {
            List<DTHUONGHIEU> lst = db.DTHUONGHIEUs.OrderBy(x => x.NAME).ToList();
            return JObject.FromObject(lst);
        }

        private JObject register(JObject param, ref string error)
        {
            JObject data = null;
            string hovaten = ConvertTo.String(param["hovaten"]);
            string taikhoan = ConvertTo.String(param["taikhoan"]);
            string matkhau = ConvertTo.String(param["matkhau"]);
            //kiểm tra trùng tài khoản
            DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.TAIKHOAN == taikhoan).FirstOrDefault();
            if (khRow != null)
            {
                error = "Tài khoản đã tồn tại";
            }
            else
            {
                khRow = new DKHACHHANG();
                khRow.TAIKHOAN = taikhoan;
                khRow.MATKHAU = matkhau;
                khRow.NAME = hovaten;
                khRow.AVATAR = "";
                khRow.ID = Guid.NewGuid().ToString();
                db.DKHACHHANGs.Add(khRow);
                db.SaveChanges();
                //data = JObject.FromObject(khRow);
            }
            return data;
        }

        private JObject getUserInfo(JObject param, ref string error)
        {
            JObject data = null;
            string DKHACHHANGID = ConvertTo.String(param["ID"]);

            DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.ID == DKHACHHANGID).FirstOrDefault();
            if (khRow == null)
            {
                error = "Thông tin đăng nhập không đúng";
            }
            else
            {
                khRow.TDONHANGs = null;
                if (ConvertTo.String(khRow.AVATAR).Length == 0)
                {
                    khRow.AVATAR = "/Images/Upload/DKHACHHANG/noavatar.jpg";
                }
                data = JObject.FromObject(khRow);
            }

            return data;
        }

        private JObject login(JObject param, ref string error)
        {
            JObject data = null;
            string username = ConvertTo.String(param["taikhoan"]);
            string password = ConvertTo.String(param["matkhau"]);

            DKHACHHANG khRow = db.DKHACHHANGs.Where(x=>x.MATKHAU == password && (x.TAIKHOAN == username || x.EMAIL == username || x.DIENTHOAI == username)).FirstOrDefault();
            if (khRow == null)
            {
                error = "Thông tin đăng nhập không đúng";
            }
            else
            {
                khRow.TDONHANGs = null;
                if (ConvertTo.String(khRow.AVATAR).Length == 0)
                {
                    khRow.AVATAR = "/Images/Upload/DKHACHHANG/noavatar.jpg";
                }
                data = JObject.FromObject(khRow);
            }

            return data;
        }
    }

    class ParamResult {
        public string cmdtype { set; get; }
        public JObject param { set; get; }
    }

    class BaseResult {

        public BaseResult() {
            isSuccess = false;
            message = "";
            data = null;
        }

        public bool isSuccess { set; get; }
        public string message { set; get; }
        public JObject data { set; get; }
    }
}