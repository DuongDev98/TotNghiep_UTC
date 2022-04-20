using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Utils
{
    public class ConvertTo
    {
        public static string String(object o)
        {
            if (o == null) return "";
            return o.ToString();
        }
        public static int Int(object o)
        {
            if (o == null) return 0;
            int kq;
            if (int.TryParse(o.ToString(), out kq))
            {
                return kq;
            }
            return 0;
        }
        public static decimal Decimal(object o)
        {
            if (o == null) return 0;
            decimal kq;
            if (decimal.TryParse(o.ToString(), out kq))
            {
                return kq;
            }
            return 0;
        }
        public static double Double(object o)
        {
            if (o == null) return 0;
            double kq;
            if (double.TryParse(o.ToString(), out kq))
            {
                return kq;
            }
            return 0;
        }
        public static DateTime Date(object o)
        {
            if (o == null) return new DateTime(1,0,0);
            DateTime kq;
            if (DateTime.TryParse(o.ToString(), out kq))
            {
                return kq;
            }
            return new DateTime(1, 0, 0);
        }
    }
}