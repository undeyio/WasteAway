﻿using System.Web.Mvc;
using WasteAway.Models;
using WasteAway.ViewModels;

namespace WasteAway.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRoute()
        {
            var model = new RouteViewModel(_context)
            {
                Trucks = _context.Trucks
            };

            model.AssignPickups();
            return View(model);
        }
    }
}