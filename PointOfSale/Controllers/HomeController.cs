using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PointOfSale.Helper;
using PointOfSale.Models;

namespace Shop_Management_System.Controllers
{
    public class HomeController : Controller
    {
        PosEntities db = new PosEntities();
        public ActionResult Index()
        {
            var dataList = db.Products.Where(x => x.IsActive == true);
            return View(dataList);
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

       

    }
}