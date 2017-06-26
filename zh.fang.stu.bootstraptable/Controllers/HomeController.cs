using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace zh.fang.stu.bootstraptable.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult Search()
        {
            var itemes = new List<object>();
            for (int i = 0; i < 100; i++)
            {
                itemes.Add(new { name = (new Random(Guid.NewGuid().GetHashCode())).Next(0, int.MaxValue), status = 0, participationCounts = 100, totalCounts = 1000, startTime = DateTime.Now.ToShortTimeString(), id = (new Random(Guid.NewGuid().GetHashCode())).Next(0, int.MaxValue) });
            }


            return Json(new { total=100, rows=itemes.ToArray() }, JsonRequestBehavior.AllowGet);
        }
    }
}