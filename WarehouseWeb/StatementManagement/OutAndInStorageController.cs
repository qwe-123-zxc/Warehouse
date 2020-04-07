using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.StatementManagement
{
    /// <summary>
    /// 出入库报表页面
    /// </summary>
    public class OutAndInStorageController : Controller
    {
        // GET: OutAndInStorage
        public ActionResult Index()
        {
            return View();
        }
    }
}