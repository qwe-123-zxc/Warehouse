using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.Controllers
{
    public class KuweiController : Controller
    {
        // GET: Kuwei
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult yemian() {
            return View();
        }
    }
}