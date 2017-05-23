using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace zh.fang.stu.callback.client.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var request = System.Web.HttpContext.Current.Request;
            var url = $"{request.Url.Scheme}://{request.Url.Host}:{request.Url.Port}/Home/{nameof(callback)}";
            return Redirect($"http://localhost:57401?url={url}");
        }
        
        public ActionResult Callback(string url)
        {
            ViewBag.Url = url;
            return View();
        }
    }
}
