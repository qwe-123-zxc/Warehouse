using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class CheckStocksController : Controller
    {
        /// <summary>
        /// 盘点管理
        /// </summary>
        /// <returns></returns>
        // GET: CheckStocks
        public ActionResult ListCheckStock()
        {
            return View();
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}