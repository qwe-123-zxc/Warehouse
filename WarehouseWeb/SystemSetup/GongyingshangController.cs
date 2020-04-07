using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.Controllers
{
    public class GongyingshangController : Controller
    {
        // GET: Gongyingshang
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult yemian() {
            return View();
        }
    }
}