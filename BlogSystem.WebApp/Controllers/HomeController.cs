﻿using System.Web.Mvc;

namespace BlogSystem.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("List", "Article");
        }
    }
}