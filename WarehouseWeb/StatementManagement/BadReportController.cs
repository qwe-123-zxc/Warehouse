using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.StatementManagement
{
    /// <summary>
    /// 报损报表页面
    /// </summary>
    public class BadReportController : Controller
    {
        // GET: Bad
        public ActionResult Index()
        {
            return View();
        }
    }
}