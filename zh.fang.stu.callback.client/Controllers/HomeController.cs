using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        [Route("")]
        public ActionResult PostRedirect()
        {
            return View();
        }
        
        public ActionResult PostRedirectSubmit()
        {
            var stream = Request.GetBufferedInputStream();
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            
            //var reader = new StreamReader(stream);
            //var str = reader.ReadToEnd();
            //var buffer = Encoding.UTF8.GetBytes(str);
            var data = Convert.ToBase64String(memStream.ToArray());
            return Redirect($"http://localhost:57401/notify?data={data}");
        }

        [Route("callback")]
        [Route("home/callback")]
        public ActionResult Callback(string url, string data)
        {
            ViewBag.Url = url;
            var buffer = Convert.FromBase64String(data);
            data = Encoding.UTF8.GetString(buffer);
            ViewBag.data = data;
            return View();
        }
    }
}
