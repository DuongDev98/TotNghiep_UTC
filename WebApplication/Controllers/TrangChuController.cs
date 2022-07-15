using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TrangChuController : Controller
    {
        DOANEntities db = new DOANEntities();
        public ActionResult Index()
        {
            //SinhDuLieuNhaCungCap();
            //SinhDuLieuKhachHang();
            //SinhDuLieuGiaoDich();
            return View();
        }

        private void SinhDuLieuGiaoDich()
        {
            Random rd = new Random();
            List<DNHACUNGCAP> lstNhaCungCap = db.DNHACUNGCAPs.ToList();
            List<DKHACHHANG> lstKhachHang = db.DKHACHHANGs.ToList();
            List<DMATHANG> lstMatHang = db.DMATHANGs.ToList();
            Dictionary<string, int> dicMatHang;
            //Mỗi tháng nhập 60 cái, bán 70% của số nhập
            for (int i = 1; i <= 6; i++)
            {
                dicMatHang = new Dictionary<string, int>();
                //Nhập đầu tháng
                DateTime ngayNhap = new DateTime(2022, i, 1);
                TDONHANG dhRow = new TDONHANG();
                dhRow.ID = Guid.NewGuid().ToString();
                dhRow.NGAY = ngayNhap;
                dhRow.NAME = NhapKhoController.GenCodeHD(db, "NK", 1);
                dhRow.LOAI = 1;
                dhRow.TILEGIAMGIA = 0;
                dhRow.TIENHANG = 0;
                dhRow.TIENGIAMGIA = 0;
                dhRow.TONGCONG = 0;
                dhRow.DNHACUNGCAPID = lstNhaCungCap[rd.Next(0, lstNhaCungCap.Count)].ID;
                db.TDONHANGs.Add(dhRow);
                db.SaveChanges();
                //Thêm chi tiết

                int soSpCuaPhieu = 15, slNhap = 10, slBan = 8;
                for (int j = 0; j < soSpCuaPhieu; j++)
                {
                    DMATHANG mhRow = lstMatHang[rd.Next(0, lstMatHang.Count)];
                    TDONHANGCHITIET ctRow = ApiController.ThemMatHangVaoHoaDon(mhRow.ID, slNhap, mhRow.GIANHAP??0, dhRow);
                    dhRow.TIENHANG += ctRow.THANHTIEN;
                    db.TDONHANGCHITIETs.Add(ctRow);
                    if (dicMatHang.ContainsKey(mhRow.ID)) dicMatHang[mhRow.ID] += 10;
                    else dicMatHang.Add(mhRow.ID, 10);
                }
                dhRow.TONGCONG = dhRow.TIENHANG;
                db.Entry(dhRow);
                db.SaveChanges();

                //thêm hóa đơn
                foreach (string key in dicMatHang.Keys)
                {
                    DMATHANG mhRow = db.DMATHANGs.Find(key);
                    DKHACHHANG khRow = lstKhachHang[rd.Next(0, lstKhachHang.Count)];

                    TDONHANG hdRow = new TDONHANG();
                    hdRow.ID = Guid.NewGuid().ToString();
                    hdRow.NAME = HoaDonController.GenCode(db, "HD");
                    hdRow.NGAY = new DateTime(2022, i, rd.Next(1, 28));
                    hdRow.LOAI = 0;
                    hdRow.DKHACHHANGID = khRow.ID;
                    hdRow.TENNGUOINHAN = khRow.NAME;
                    hdRow.DIENTHOAI = khRow.DIENTHOAI??"";
                    hdRow.DIACHI = khRow.DIACHI??"";
                    hdRow.GHICHU = "";
                    hdRow.DTINHTHANHID = khRow.DTINHTHANHID;
                    hdRow.DQUANHUYENID = khRow.DQUANHUYENID;
                    hdRow.DPHUONGXAID = khRow.DPHUONGXAID;
                    hdRow.HINHTHUCTHANHTOAN = 0;
                    hdRow.TILEGIAMGIA = 0;
                    hdRow.TIENHANG = mhRow.GIABAN;
                    dhRow.TRANGTHAI = 3;

                    //tính toán
                    List<TDONHANGCHITIET> chitiets = new List<TDONHANGCHITIET>();
                    TDONHANGCHITIET ctRow = ApiController.ThemMatHangVaoHoaDon(mhRow.ID, slBan, mhRow.GIANHAP ?? 0, dhRow);
                    chitiets.Add(ctRow);

                    hdRow.TIENGIAMGIA = hdRow.TILEGIAMGIA == 0 ? 0 : (hdRow.TIENHANG * hdRow.TILEGIAMGIA / 100);
                    hdRow.TONGCONG = hdRow.TIENHANG - hdRow.TIENGIAMGIA;
                    //luu hoa don
                    db.TDONHANGs.Add(hdRow);
                    db.SaveChanges();
                    //luu chi tiet
                    db.TDONHANGCHITIETs.AddRange(chitiets);
                    db.SaveChanges();
                }
            }
        }

        string[] hos = "Nguyễn|Phạm|Lê|Trần|Hoàng|Huỳnh|Vũ|Võ|Đặng|Bùi|Đỗ|Hồ|Ngô|Dương|Lý|Tống|Phí|Đào|Đoàn|Vương|Trịnh|Trương|Đinh|Lâm|Phùng|Mai|Tô|Trương|Hà|Tạ".Split(new char[] { '|' });
        string[] ten = "Anh|Dũng|Hà|Chơn|Nhơn|Yến|Long|Vân|Tuyên|Quyền|Thập|Việt|Hệ|Minh|Thủy|Oanh|Tạo|điền|Khiết|Triết|Hòa|Linh|Chí|Thùy|Chiến|Do|Hùng|Thành|Lộc|Sơn|Thoa|Trang|Huy|Châu|Nội|Mai|Xuân|Đắc|Pháp|Sinh|Trì|Chương|Khoái|Nguyễn|Nguyên|Giang|Tuấn|Cúc|Thể|Tú|Thức|Đông|Phòng|Nam|Dụ|Vinh|Khánh|Tựu|Tụ|Toản|Hoài|Thắng|Dương|Thuận|Tân|Đức|Ngọc|Rạng|Kế|Bình|Chi|Hoàng|Lạc|Thản|Khôi|Nhân|Vũ|Điệp|Diệp|Nghĩa|Viện|Viên|Nhượng|Ánh|Ngữ|Hiếu|Phong|Phùng|Ông|Bách|Chuyên|Dung|Tình|Nhuận|Báu|Bảo|Tráng|Phước|Chữ|Duy|Quỳnh|Hằng|Uyển|Quyển|Kỳ|Quang|Tĩnh|Phú|Tùng|Lịch|Quý|Lữ|Duyên|Diệu|Huyền|Đồng|Thông|Thụ|Đê|Huân|Đễ|Hào|Hảo|Khanh|Kiên|Khoa|Sỹ|Quân|Yên|Toán|Đăng|Cung|Lợi|Thạnh|Vi|Lê|Tích|Tâm|Thảo|Trí|An|Tín|Lương|Hóa|Hồng".Split(new char[] { '|' });
        string[] tenDem = "Việt|Văn|Thị|Quốc|Nguyên|Bá|Long|Mai|Huy|Đỗ|Trung|Anh|Hồng|Khánh|Công|Mạnh|Tấn|Hữu|Xuân|Dương|Ngọc|Tất|Phúc|Trọng|Bích|Quang|Đăng|Nhật|Minh|Thanh|Đình|Thái".Split(new char[] { '|' });
        string[] emails = new string[] { "gmail.com", "yahoo.com", "yahoo.com.vn", "vnn.vn", "facebook.com", "ymail.com", "hotmail.com" };
        private void SinhDuLieuKhachHang()
        {
            //sinh 300 khách hàng
            Random rd = new Random();
            List<DTINHTHANH> lstTinhThanh = db.DTINHTHANHs.ToList();
            //sinh 12 nha cung cap
            for (int i = 0; i < 300; i++)
            {
                DKHACHHANG khRow = new DKHACHHANG();
                khRow.ID = Guid.NewGuid().ToString();
                khRow.NAME = hos[rd.Next(0, hos.Length)];
                khRow.NAME += " " + tenDem[rd.Next(0, tenDem.Length)];
                khRow.NAME += " " + ten[rd.Next(0, ten.Length)];
                string username = RemoveUnicode(khRow.NAME).Replace(" ", "");
                khRow.TAIKHOAN = username;
                khRow.MATKHAU = username;
                khRow.EMAIL = username + "@" + emails[rd.Next(0, emails.Length)];
                //random tinh thanh
                khRow.DTINHTHANHID = lstTinhThanh[rd.Next(0, lstTinhThanh.Count)].ID;
                List<DQUANHUYEN> lstQuanHuyen = db.DQUANHUYENs.Where(x => x.DTINHTHANHID == khRow.DTINHTHANHID).ToList();
                khRow.DQUANHUYENID = lstQuanHuyen[rd.Next(0, lstQuanHuyen.Count)].ID;
                List<DPHUONGXA> lstPhuongXa = db.DPHUONGXAs.Where(x => x.DQUANHUYENID == khRow.DQUANHUYENID).ToList();
                khRow.DPHUONGXAID = lstPhuongXa[rd.Next(0, lstPhuongXa.Count)].ID;
                db.DKHACHHANGs.Add(khRow);
            }
            db.SaveChanges();
        }
        private void SinhDuLieuNhaCungCap()
        {
            Random rd = new Random();
            //sinh 12 nha cung cap
            for (int i = 0; i < 12; i++)
            {
                DNHACUNGCAP nccRow = new DNHACUNGCAP();
                //ID. NAME. DIENTHOAI. DIACHI. EMAIL
                nccRow.ID = Guid.NewGuid().ToString();
                nccRow.NAME = hos[rd.Next(0, hos.Length)];
                nccRow.NAME += " " + tenDem[rd.Next(0, tenDem.Length)];
                nccRow.NAME += " " + ten[rd.Next(0, ten.Length)];
                nccRow.EMAIL = RemoveUnicode(nccRow.NAME).Replace(" ", "") + "@" + emails[rd.Next(0, emails.Length)];
                db.DNHACUNGCAPs.Add(nccRow);
            }
            db.SaveChanges();
        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};
                    string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }
    }
}