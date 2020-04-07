using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.StatementManagement
{
    /// <summary>
    /// 出库报表页面
    /// </summary>
    public class OutStorageReportController : Controller
    {
        // GET: OutStorage
        public ActionResult Index()
        {
            return View();
        }
    }
}