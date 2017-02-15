using System;
using Common.Table;
using System.Web.Mvc;

namespace Consumer.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.Tracker = "0000";
            return View();
        }

        [HttpPost]
        public ActionResult Index(ServiceRequest request)
        {
            ViewBag.Title = "Home Page";
            var tracker = Guid.NewGuid();
            request.Tracker = tracker;
            ViewBag.Tracker = tracker.ToString();
            return View();
        }
    }
}