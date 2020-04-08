using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.Controllers
{
    public class WarehouseController : Controller
    {
        // GET: Warehouse
        //首页
        public ActionResult Index()
        {
            return View();
        }

        //登陆页面
        public ActionResult LoginPage()
        {
            return View();
        }
    }
}