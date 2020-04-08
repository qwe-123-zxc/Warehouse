using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class ReturnOrderStocksController : Controller
    {
        /// <summary>
        /// 退货管理
        /// </summary>
        /// <returns></returns>
        // GET: ReturnOrderStocks
        public ActionResult ListReturnOrderStock()
        {
            return View();
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}