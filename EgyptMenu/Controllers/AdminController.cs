using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EgyptMenu.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Restaurants()
        {
            return View();
        }
        public ActionResult Plans()
        {
            return View();
        }
        public ActionResult LangTranslations()
        {
            return View();
        }
        public ActionResult Env()
        {
            return View();
        }
        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult CreateRestaurant()
        {
            return View();
        }
        public ActionResult CreatePlan()
        {
            return View();
        }
        public ActionResult Systemstatus()
        {
            return View();
        }

    }
}