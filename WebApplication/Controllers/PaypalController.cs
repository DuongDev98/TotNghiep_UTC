using PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class PaypalController : Controller
    {
        string clientId = "AT-sB251aN9T0rAHHpGMXpDOvHY-UpbCOzKU2SBy5vTr5lQK37k47MLROJxftoDfp_CKcR9yMm0tVm6F";
        string secretKey = "EE3HzspjKx4QpsgzaoyRCWzKFCJX7gQNblcWi-fIUWxxja4aTjsizRARKpll6DVLXSV5D2gbDLzBhOYY";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentMethod()
        {
            try
            {
                Dictionary<string, string> config = new Dictionary<string, string>();
                config.Add("mode", "sandbox");
                config.Add("clientId", clientId);
                config.Add("clientSecret", secretKey);
                string accessToken = new OAuthTokenCredential(config).GetAccessToken();
                var apiContext = new APIContext(accessToken);

                // Make an API call
                var payment = Payment.Create(apiContext, new Payment
                {
                    intent = "sale",
                    payer = new Payer
                    {
                        payment_method = "paypal"
                    },
                    transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        description = "Transaction description.",
                        invoice_number = "001",
                        amount = new Amount
                        {
                            currency = "USD",
                            total = "1",
                            details = new Details()
                        }
                    }
                },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = "http://localhost:44361/Paypal/Success",
                        cancel_url = "http://localhost:44361/Paypal/Error"
                    }
                });

                //lấy link redirect
                string urlRedirect = "";
                foreach (var link in payment.links)
                {
                    if (link.method == "REDIRECT")
                    {
                        urlRedirect = link.href;
                    }
                }
                return Redirect(urlRedirect);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View("Index");
            }
        }

        public ActionResult Success()
        {
            return Content("Payment success");
        }
        public ActionResult Error()
        {
            return Content("Payment error");
        }
    }
}