using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class BadReportsController : Controller
    {
        /// <summary>
        /// 报损管理
        /// </summary>
        /// <returns></returns>
        // GET: BadReports
        public ActionResult ListBadReport()
        {
            return View();
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}