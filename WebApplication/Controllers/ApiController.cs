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
                            //case "duLieuDiaDiem": kq.data = layDuLieuDiaDiem(attr.param, ref error); break;
                            case "capNhatThongTin": kq.data = capNhatThongTin(attr.param, ref error); break;
                            case "uploadavatar": kq.data = uploadAvatar(attr.param, ref error); break;
                            case "thongTinMatHang": kq.data = thongTinMatHang(attr.param, ref error); break;
                            case "thucHienThanhToan": kq.data = thucHienThanhToan(attr.param, ref error); break;
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

        private JObject thucHienThanhToan(JObject param, ref string error)
        {
            TDONHANG dhRow = new TDONHANG();
            dhRow.ID = Guid.NewGuid().ToString();
            dhRow.NAME = HoaDonController.GenCode(db, "HD");
            dhRow.LOAI = 0;
            dhRow.DKHACHHANGID = ConvertTo.String(param["DKHACHHANGID"]);
            dhRow.TENNGUOINHAN = ConvertTo.String(param["TENNGUOINHAN"]);
            dhRow.DIENTHOAI = ConvertTo.String(param["DIENTHOAI"]);
            dhRow.DIACHI = ConvertTo.String(param["DIACHI"]);
            dhRow.GHICHU = ConvertTo.String(param["GHICHU"]);
            dhRow.DTINHTHANHID = ConvertTo.String(param["DTINHTHANHID"]);
            dhRow.DQUANHUYENID = ConvertTo.String(param["DQUANHUYENID"]);
            dhRow.DPHUONGXAID = ConvertTo.String(param["DPHUONGXAID"]);
            dhRow.TRANGTHAI = 0;
            dhRow.TIENHANG = 0;

            //tính toán
            List<TDONHANGCHITIET> chitiets = new List<TDONHANGCHITIET>();
            JArray arrChiTiet = JArray.FromObject(param["TDONHANGCHITIETs"]);
            foreach (JObject o in arrChiTiet)
            {
                TDONHANGCHITIET chiTietRow = new TDONHANGCHITIET();
                chiTietRow.ID = Guid.NewGuid().ToString();
                chiTietRow.DMATHANGID = ConvertTo.String(o["DMATHANGID"]);
                chiTietRow.DONGIA = ConvertTo.Decimal(o["DONGIA"]);
                chiTietRow.SOLUONG = ConvertTo.Decimal(o["SOLUONG"]);
                chiTietRow.TDONHANGID = dhRow.ID;
                chiTietRow.THANHTIEN = chiTietRow.SOLUONG * chiTietRow.DONGIA;
                dhRow.TIENHANG += chiTietRow.THANHTIEN;
                chitiets.Add(chiTietRow);
            }
            dhRow.TONGCONG = dhRow.TIENHANG;
            //luu hoa don
            db.TDONHANGs.Add(dhRow);
            db.SaveChanges();
            //luu chi tiet
            db.TDONHANGCHITIETs.AddRange(chitiets);
            db.SaveChanges();
            return null;
        }

        private JObject thongTinMatHang(JObject param, ref string error)
        {
            JObject kq = new JObject();
            string DMATHANGID = ConvertTo.String(param["ID"]);
            DMATHANG mhRow = db.DMATHANGs.Find(DMATHANGID);
            mhRow.MOTA = HttpUtility.HtmlDecode(mhRow.MOTA);
            string imgUrl = "/Images/Upload/DMATHANG/";
            if (mhRow.DANHSANPHAMs != null)
            {
                foreach (DANHSANPHAM temp in mhRow.DANHSANPHAMs)
                {
                    temp.LINK = imgUrl + temp.LINK;
                    while (temp.DMATHANG != null) temp.DMATHANG = null;
                }
            }
            else
            {
                //thêm ảnh mặc định
                DANHSANPHAM temp = new DANHSANPHAM();
                temp.LINK = imgUrl + "/noavatar.jpg";
                List<DANHSANPHAM> lst = new List<DANHSANPHAM>() { temp };
                mhRow.DANHSANPHAMs = lst;
            }
            mhRow.DKHUYENMAICHITIETs.Clear();
            mhRow.TDONHANGCHITIETs.Clear();
            while (mhRow.DNHOMMATHANG != null) mhRow.DNHOMMATHANG = null;
            while (mhRow.DTHUONGHIEU != null) mhRow.DTHUONGHIEU = null;
            kq = JObject.FromObject(mhRow);
            return kq;
        }

        private JObject uploadAvatar(JObject param, ref string error)
        {
            JObject kq = new JObject();
            //upload avatar
            JArray assets = JArray.FromObject(param["assets"]);
            if (assets.Count > 0)
            {
                //lấy ra file đầu tiên
                JObject file = JObject.FromObject(assets[0]);
                string base64 = ConvertTo.String(file["base64"]);
                //upload anh
                string DKHACHHANGID = ConvertTo.String(param["khachhangid"]);
                DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.ID == DKHACHHANGID).FirstOrDefault();
                if (khRow == null)
                {
                    error = "Có lỗi trong quá trình cập nhật ảnh đại diện";
                }
                else
                {
                    //Xóa ảnh cũ trước
                    string temp = khRow.AVATAR;
                    if (temp != null && temp.Length > 0) FileUtils.Delete(Server, "DKHACHHANG", temp);
                    //cập nhật ảnh mới
                    temp = FileUtils.Upload(Server, "DKHACHHANG", base64);
                    khRow.AVATAR = temp;
                    db.Entry(khRow);
                    db.SaveChanges();
                    kq["img"] = "/Images/Upload/DKHACHHANG/" + temp;
                }
            }
            else
            {
                error = "Không có ảnh nào được chọn";
            }
            return kq;
        }

        private JObject capNhatThongTin(JObject param, ref string error)
        {
            string id = ConvertTo.String(param["id"]);
            string name = ConvertTo.String(param["name"]);
            string dienthoai = ConvertTo.String(param["dienthoai"]);
            string email = ConvertTo.String(param["email"]);
            string tinhthanhid = ConvertTo.String(param["tinhthanhid"]);
            string quanhuyenid = ConvertTo.String(param["quanhuyenid"]);
            string phuongxaid = ConvertTo.String(param["phuongxaid"]);
            string diachi = ConvertTo.String(param["diachi"]);
            //kiểm tra trùng điện thoại
            DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.ID != id && x.EMAIL == email).FirstOrDefault();
            if (khRow != null) error = "Email trùng với 1 khách hàng khác";
            //kiểm tra trùng email
            if (error.Length == 0)
            {
                khRow = db.DKHACHHANGs.Where(x => x.ID != id && x.DIENTHOAI == dienthoai).FirstOrDefault();
                if (khRow != null) error = "Điện thoại trùng với 1 khách hàng khác";
            }

            if (error.Length > 0)
            {
                error += ", không thể cập nhật dữ liệu";
                return null;
            }

            khRow = db.DKHACHHANGs.Where(x => x.ID == id).FirstOrDefault();
            if (khRow == null)
            {
                error = "Có lỗi trong quá trình cập nhật thông tin";
            }
            else
            {
                khRow.NAME = name;
                khRow.DIENTHOAI = dienthoai;
                khRow.DIACHI = diachi;
                khRow.EMAIL = email;
                khRow.DTINHTHANHID = tinhthanhid;
                khRow.DQUANHUYENID = quanhuyenid;
                khRow.DPHUONGXAID = phuongxaid;
                db.Entry(khRow);
                db.SaveChanges();
            }
            return null;
        }

        private JObject layDanhSachPhuongXa(JObject param, ref string error)
        {
            string DQUANHUYENID = ConvertTo.String(param["ID"]);
            JArray arr = new JArray();
            List<DPHUONGXA> lst = db.DPHUONGXAs.Where(x => x.DQUANHUYENID == DQUANHUYENID).OrderBy(x => x.NAME).ToList();
            foreach (DPHUONGXA item in lst)
            {
                JObject o = new JObject();
                o["ID"] = item.ID;
                o["CODE"] = item.CODE;
                o["NAME"] = item.NAME;
                o["DQUANHUYENID"] = item.DQUANHUYENID;
                arr.Add(o);
            }
            JObject kq = new JObject();
            kq["arr"] = arr;
            return kq;
        }

        private JObject layDanhSachQuanHuyen(JObject param, ref string error)
        {
            string DTINHTHANHID = ConvertTo.String(param["ID"]);
            JArray arr = new JArray();
            List<DQUANHUYEN> lst = db.DQUANHUYENs.Where(x => x.DTINHTHANHID == DTINHTHANHID).OrderBy(x => x.NAME).ToList();
            foreach (DQUANHUYEN item in lst)
            {
                JObject o = new JObject();
                o["ID"] = item.ID;
                o["CODE"] = item.CODE;
                o["NAME"] = item.NAME;
                o["DTINHTHANHID"] = item.DTINHTHANHID;
                arr.Add(o);
            }
            JObject kq = new JObject();
            kq["arr"] = arr;
            return kq;
        }

        private JObject layDanhSachTinhThanh(JObject param, ref string error)
        {
            JArray arr = new JArray();
            List<DTINHTHANH> lst = db.DTINHTHANHs.OrderBy(x => x.NAME).ToList();
            foreach (DTINHTHANH item in lst)
            {
                JObject o = new JObject();
                o["ID"] = item.ID;
                o["CODE"] = item.CODE;
                o["NAME"] = item.NAME;
                arr.Add(o);
            }
            JObject kq = new JObject();
            kq["arr"] = arr;
            return kq;
        }

        public static void exportDuLieuDiaDiem(DOANEntities db)
        {
            JObject kq = new JObject();
            JArray arrTinhThanh = new JArray();
            JArray arrQuanHuyen = new JArray();
            JArray arrPhuongXa = new JArray();
            //dữ liệu tỉnh thành
            List<DTINHTHANH> lstTinhThanh = db.DTINHTHANHs.OrderBy(x => x.NAME).ToList();
            foreach (DTINHTHANH item in lstTinhThanh)
            {
                JObject o = new JObject();
                o["ID"] = item.ID;
                o["CODE"] = item.CODE;
                o["NAME"] = item.NAME;
                arrTinhThanh.Add(o);
            }
            //JArray arrTinhThanh = JArray.FromObject(lstTinhThanh);
            //dữ liệu quận huyện
            List<DQUANHUYEN> lstQuanHuyen = db.DQUANHUYENs.OrderBy(x => x.NAME).ToList();
            foreach (DQUANHUYEN item in lstQuanHuyen)
            {
                //item.DKHACHHANGs.Clear();
                //item.DPHUONGXAs.Clear();
                //item.TDONHANGs.Clear();
                //while (item.DTINHTHANH != null) item.DTINHTHANH = null;

                JObject o = new JObject();
                o["ID"] = item.ID;
                o["CODE"] = item.CODE;
                o["NAME"] = item.NAME;
                o["DTINHTHANHID"] = item.DTINHTHANHID;
                arrQuanHuyen.Add(o);
            }
            //JArray arrQuanHuyen = JArray.FromObject(lstQuanHuyen);
            //dữ liệu phường xã
            List<DPHUONGXA> lstPhuongXa = db.DPHUONGXAs.OrderBy(x => x.NAME).ToList();
            foreach (DPHUONGXA item in lstPhuongXa)
            {
                //item.DKHACHHANGs.Clear();
                //item.TDONHANGs.Clear();
                //while (item.DQUANHUYEN != null) item.DQUANHUYEN = null;

                JObject o = new JObject();
                o["ID"] = item.ID;
                o["CODE"] = item.CODE;
                o["NAME"] = item.NAME;
                o["DQUANHUYENID"] = item.DQUANHUYENID;
                arrPhuongXa.Add(o);
            }
            //JArray arrPhuongXa = JArray.FromObject(lstPhuongXa);

            StreamWriter streamWriter = new StreamWriter(@"E:\dulieutinhthanh.json");
            streamWriter.WriteLine(JsonConvert.SerializeObject(arrTinhThanh));
            streamWriter = new StreamWriter(@"E:\dulieuquanhuyen.json");
            streamWriter.WriteLine(JsonConvert.SerializeObject(arrQuanHuyen));
            streamWriter = new StreamWriter(@"E:\dulieuphuongxa.json");
            streamWriter.WriteLine(JsonConvert.SerializeObject(arrPhuongXa));
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
            string DNHOMMATHANGID = ConvertTo.String(param["nhomId"]);
            string DTHUONGHIEUID = ConvertTo.String(param["thuongHieuId"]);

            JArray arr = new JArray();
            IQueryable<DMATHANG> lst = db.DMATHANGs.Where(x=>x.NAME.Contains(s) || s == "");
            lst = lst.Where(x => x.DNHOMMATHANGID == DNHOMMATHANGID || DNHOMMATHANGID == "");
            lst = lst.Where(x => x.DTHUONGHIEUID == DTHUONGHIEUID || DTHUONGHIEUID == "");
            foreach (DMATHANG it in lst)
            {
                string avatar = "";
                if (it.DANHSANPHAMs != null)
                {
                    DANHSANPHAM anh = it.DANHSANPHAMs.FirstOrDefault();
                    if (anh != null)
                    {
                        avatar = anh.LINK;
                    }
                }
                if (avatar.Length == 0)
                {
                    avatar = "noavatar.jpg";
                }
                it.DANHSANPHAMs.Clear();
                it.DKHUYENMAICHITIETs.Clear();
                it.TDONHANGCHITIETs.Clear();
                while (it.DNHOMMATHANG != null) it.DNHOMMATHANG = null;
                while (it.DTHUONGHIEU != null) it.DTHUONGHIEU = null;

                JObject oMatHang = JObject.FromObject(it);
                oMatHang["AVATAR"] = "/Images/Upload/DMATHANG/" + avatar;
                arr.Add(oMatHang);
            }
            JObject item = new JObject();
            item["arr"] = arr;
            return item;
        }

        private JObject layDanhSachNhom(JObject param, ref string error)
        {
            JObject kq = new JObject();
            JArray lstNhoms = new JArray();
            List<DNHOMMATHANG> lst = db.DNHOMMATHANGs.OrderBy(x => x.NAME).ToList();
            foreach (DNHOMMATHANG it in lst)
            {
                it.DMATHANGs.Clear();
                JObject o = JObject.FromObject(it);
                JArray arr = new JArray();
                //lấy danh sách mặt hàng, mỗi nhóm lấy 5 sản phẩm
                List<DMATHANG> lstMatHang = db.DMATHANGs.Where(x => x.DNHOMMATHANGID == it.ID).Take(4).ToList();
                foreach (DMATHANG mhRow in lstMatHang)
                {
                    string avatar = "";
                    if (mhRow.DANHSANPHAMs != null)
                    {
                        DANHSANPHAM anh = mhRow.DANHSANPHAMs.FirstOrDefault();
                        if (anh != null)
                        {
                            avatar = anh.LINK;
                        }
                    }
                    if (avatar.Length == 0)
                    {
                        avatar = "noavatar.jpg";
                    }

                    while (mhRow.DANHSANPHAMs != null) mhRow.DANHSANPHAMs = null;
                    while (mhRow.DKHUYENMAICHITIETs != null) mhRow.DKHUYENMAICHITIETs = null;
                    while (mhRow.TDONHANGCHITIETs != null) mhRow.TDONHANGCHITIETs = null;
                    while (mhRow.DNHOMMATHANG != null) mhRow.DNHOMMATHANG = null;
                    while (mhRow.DTHUONGHIEU != null) mhRow.DTHUONGHIEU = null;
                    JObject oMatHang = JObject.FromObject(mhRow);
                    oMatHang["AVATAR"] = "/Images/Upload/DMATHANG/" + avatar;
                    arr.Add(oMatHang);
                }
                o["DMATHANGs"] = arr;
                lstNhoms.Add(o);
            }
            kq["arr"] = lstNhoms;
            return kq;
        }

        private JObject layDanhSachThuongHieu(JObject param, ref string error)
        {
            List<DTHUONGHIEU> lst = db.DTHUONGHIEUs.OrderBy(x => x.NAME).ToList();
            foreach (DTHUONGHIEU it in lst) it.DMATHANGs.Clear();
            JObject arr = new JObject();
            arr["arr"] = JArray.FromObject(lst);
            return arr;
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
                khRow.TDONHANGs.Clear();
                string img = "/Images/Upload/DKHACHHANG/";
                if (ConvertTo.String(khRow.AVATAR).Length == 0) khRow.AVATAR = img + "noavatar.jpg";
                else khRow.AVATAR = img + khRow.AVATAR;

                while (khRow.DTINHTHANH != null) khRow.DTINHTHANH = null;
                while (khRow.DQUANHUYEN != null) khRow.DQUANHUYEN = null;
                while (khRow.DPHUONGXA != null) khRow.DPHUONGXA = null;
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
                khRow.TDONHANGs.Clear();
                string img = "/Images/Upload/DKHACHHANG/";
                if (ConvertTo.String(khRow.AVATAR).Length == 0) khRow.AVATAR = img + "noavatar.jpg";
                else khRow.AVATAR = img + khRow.AVATAR;

                while (khRow.DTINHTHANH != null) khRow.DTINHTHANH = null;
                while (khRow.DQUANHUYEN != null) khRow.DQUANHUYEN = null;
                while (khRow.DPHUONGXA != null) khRow.DPHUONGXA = null;
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