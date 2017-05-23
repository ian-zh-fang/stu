using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace zh.fang.stu.callback.server.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string url)
        {
            ViewBag.Title = "Home Page";

            //return View();
            return RedirectToAction(nameof(ServerIndex), new { url = url });
        }

        public ActionResult ServerIndex(String url)
        {
            return View(new ServerModel(url));
        }

        public ActionResult Submit(string url)
        {
            if(!string.Equals(url, ServerModel.DefaultUrl))
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    var client = new System.Net.WebClient();
                    var buffer = System.Text.Encoding.UTF8.GetBytes($"url={url}");
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    client.Headers.Add("ContentLength", buffer.Length.ToString(CultureInfo.InvariantCulture));
                    // callback
                    client.UploadDataAsync(new Uri(url), "POST", buffer);
                }).ConfigureAwait(false);
            }

            // local return
            return View(new ServerModel(url));
        }

        public ActionResult Success(string url)
        {
            return Redirect(url);
            //return RedirectToAction(nameof(Index), new { url = url });
        }
    }

    public sealed class ServerModel
    {
        public const string DefaultUrl = "/";

        public ServerModel(string url)
        {
            Url = url ?? DefaultUrl;
        }

        public string Url { get; private set; }
    }
}
