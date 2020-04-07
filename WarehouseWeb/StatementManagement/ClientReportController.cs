using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.StatementManagement
{
    /// <summary>
    /// 客户报表页面
    /// </summary>
    public class ClientReportController : Controller
    {
        // GET: Client
        public ActionResult Index()
        {
            return View();
        }
    }
}