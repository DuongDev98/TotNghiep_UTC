using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

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

        private JObject register(JObject param, ref string error)
        {
            JObject data = null;
            string hovaten = param["hovaten"].ToString();
            string taikhoan = param["taikhoan"].ToString();
            string matkhau = param["matkhau"].ToString();
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

        private JObject login(JObject param, ref string error)
        {
            JObject data = null;
            string username = param["taikhoan"].ToString();
            string password = param["matkhau"].ToString();

            DKHACHHANG khRow = db.DKHACHHANGs.Where(x=>x.MATKHAU == password && (x.TAIKHOAN == username || x.EMAIL == username || x.DIENTHOAI == username)).FirstOrDefault();
            if (khRow == null)
            {
                error = "Thông tin đăng nhập không đúng";
            }
            else
            {
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