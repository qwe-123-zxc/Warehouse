using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.SystemSetup
{
    /// <summary>
    /// 权限分配页面
    /// </summary>
    public class PermissionGrantedController : Controller
    {
        // GET: PermissionGranted
        public ActionResult Index()
        {
            return View();
        }
    }
}