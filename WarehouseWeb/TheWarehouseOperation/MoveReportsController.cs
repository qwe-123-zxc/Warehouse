using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class MoveReportsController : Controller
    {
        /// <summary>
        /// 移库管理
        /// </summary>
        /// <returns></returns>
        // GET: MoveReports
        public ActionResult ListMoveReport()
        {
            return View();
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}