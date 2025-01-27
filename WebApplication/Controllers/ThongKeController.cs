﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.Controllers
{
    public class ThongKeController : Controller
    {
        DOANEntities db = new DOANEntities();

        [HttpGet]
        public ActionResult ChiTietCongNoNhaCungCap(string DNHACUNGCAPID)
        {
            ViewBag.ncc = db.DNHACUNGCAPs.ToList();
            ViewBag.nccSelect = DNHACUNGCAPID;
            string query = @"SELECT * FROM
            (
                SELECT FORMAT(NGAY, 'dd/MM/yyyy') AS NGAY, NAME, TONGCONG, TIENTHANHTOAN, CAST(0 AS DECIMAL(18,2)) AS LUYKE FROM TDONHANG WHERE LOAI = 1 AND DNHACUNGCAPID = '{0}'
                UNION
                SELECT FORMAT(NGAY, 'dd/MM/yyyy'), NAME, 0, CHI, 0 FROM TTHUCHI WHERE LOAI = 1 AND DNHACUNGCAPID = '{0}'
            ) A
            ORDER BY A.NGAY, A.NAME";
            DataTable dt = DatabaseUtils.GetTable(string.Format(query, DNHACUNGCAPID));
            if (dt.Rows.Count == 0)
            {
                ViewBag.error = "Dữ liệu giao dịch trống";
            }
            else
            {
                foreach (System.Data.DataColumn col in dt.Columns) col.ReadOnly = false;

                decimal luyKe = 0;
                foreach (DataRow row in dt.Rows)
                {
                    luyKe += ConvertTo.Decimal(row["TIENTHANHTOAN"]) - ConvertTo.Decimal(row["TONGCONG"]);
                    row["LUYKE"] = luyKe;
                }
            }
            return View(dt);
        }

        [HttpGet]
        public ActionResult DanhSachMatHangNhapTheoNgay(string fDate, string tDate)
        {
            DateTime fromDate = (fDate != null && fDate.Length > 0 ? Convert.ToDateTime(fDate) : DateTime.Now).Date;
            DateTime toDate = (tDate != null && tDate.Length > 0 ? Convert.ToDateTime(tDate) : DateTime.Now).Date;
            ViewBag.fDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.tDate = toDate.ToString("yyyy-MM-dd");
            string query = @"SELECT FORMAT(TDONHANG.NGAY, 'dd/MM/yyyy') AS NGAY, TDONHANG.NAME AS SOPHIEU,
            (SELECT NAME FROM DNHACUNGCAP WHERE DNHACUNGCAPID = DNHACUNGCAP.ID) AS DNHACUNGCAP_NAME,
            DMATHANG.CODE, DMATHANG.NAME, DONGIA, SUM(SOLUONG) AS SOLUONG, SUM(THANHTIEN) AS THANHTIEN
            FROM DMATHANG INNER JOIN TDONHANGCHITIET ON DMATHANGID = DMATHANG.ID
            INNER JOIN TDONHANG ON TDONHANGID = TDONHANG.ID AND LOAI = 1
            AND NGAY BETWEEN @FromDate AND @ToDate
            GROUP BY TDONHANG.NGAY,TDONHANG.NAME, DNHACUNGCAPID, DMATHANG.CODE, DMATHANG.NAME, DONGIA ORDER BY DMATHANG.CODE";

            Dictionary<string, object> attrs = new Dictionary<string, object>();
            attrs.Add("@FromDate", fromDate);
            attrs.Add("@ToDate", toDate);
            DataTable dt = DatabaseUtils.GetTable(query, attrs);
            if (dt.Rows.Count == 0)
            {
                ViewBag.error = "Dữ liệu giao dịch trống";
            }
            return View(dt);
        }

        [HttpGet]
        public ActionResult DanhSachMatHangDatTheoNgay(string fDate, string tDate)
        {
            DateTime fromDate = (fDate != null && fDate.Length > 0 ? Convert.ToDateTime(fDate) : DateTime.Now).Date;
            DateTime toDate = (tDate != null && tDate.Length > 0 ? Convert.ToDateTime(tDate) : DateTime.Now).Date;
            ViewBag.fDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.tDate = toDate.ToString("yyyy-MM-dd");
            string query = @"SELECT FORMAT(TDONHANG.NGAY, 'dd/MM/yyyy') AS NGAY, TDONHANG.NAME AS SOPHIEU,
            (SELECT NAME FROM DKHACHHANG WHERE DKHACHHANGID = DKHACHHANG.ID) AS DKHACHHANG_NAME,
            DMATHANG.CODE, DMATHANG.NAME, DONGIA, SUM(SOLUONG) AS SOLUONG, SUM(THANHTIEN) AS THANHTIEN
            FROM DMATHANG INNER JOIN TDONHANGCHITIET ON DMATHANGID = DMATHANG.ID
            INNER JOIN TDONHANG ON TDONHANGID = TDONHANG.ID AND LOAI = 0 AND TRANGTHAI <> 1
            AND NGAY BETWEEN @FromDate AND @ToDate
            GROUP BY TDONHANG.NGAY,TDONHANG.NAME, DKHACHHANGID, DMATHANG.CODE, DMATHANG.NAME, DONGIA ORDER BY DMATHANG.CODE";

            Dictionary<string, object> attrs = new Dictionary<string, object>();
            attrs.Add("@FromDate", fromDate);
            attrs.Add("@ToDate", toDate);
            DataTable dt = DatabaseUtils.GetTable(query, attrs);
            if (dt.Rows.Count == 0)
            {
                ViewBag.error = "Dữ liệu giao dịch trống";
            }
            return View(dt);
        }

        [HttpGet]
        public ActionResult LoiNhuanTheoNgay(string fDate, string tDate)
        {
            DateTime fromDate = (fDate != null && fDate.Length > 0 ? Convert.ToDateTime(fDate) : DateTime.Now).Date;
            DateTime toDate = (tDate != null && tDate.Length > 0 ? Convert.ToDateTime(tDate) : DateTime.Now).Date;
            ViewBag.fDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.tDate = toDate.ToString("yyyy-MM-dd");
            string query = @"SELECT FORMAT(NGAY, 'dd/MM/yyyy') AS NGAY, NAME, TONGCONG FROM TDONHANG WHERE LOAI = 1 AND NGAY BETWEEN @FromDate AND @ToDate";
            Dictionary<string, object> attrs = new Dictionary<string, object>();
            attrs.Add("@FromDate", fromDate);
            attrs.Add("@ToDate", toDate);
            DataTable dt = DatabaseUtils.GetTable(query, attrs);
            ViewBag.tblNhapKho = dt;
            decimal tongNhap = 0;
            foreach (DataRow row in dt.Rows)
            {
                tongNhap += ConvertTo.Decimal(row["TONGCONG"]);
            }

            query = @"SELECT FORMAT(NGAY, 'dd/MM/yyyy') AS NGAY, NAME, TONGCONG FROM TDONHANG WHERE LOAI = 0 AND COALESCE(TRANGTHAI, 0)<>1 AND NGAY BETWEEN @FromDate AND @ToDate";
            dt = DatabaseUtils.GetTable(query, attrs);
            ViewBag.tblHoaDon = dt;
            decimal tongXuat = 0;
            foreach (DataRow row in dt.Rows)
            {
                tongXuat += ConvertTo.Decimal(row["TONGCONG"]);
            }
            decimal loiNhuan = tongXuat - tongNhap;
            ViewBag.tongNhap = tongNhap;
            ViewBag.tongXuat = tongXuat;
            ViewBag.loiNhuan = loiNhuan;

            if (dt.Rows.Count == 0)
            {
                ViewBag.error = "Dữ liệu giao dịch trống";
            }
            return View(dt);
        }
    }
}