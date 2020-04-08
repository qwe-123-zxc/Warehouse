using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class OutStoragesController : Controller
    {
        /// <summary>
        /// 出库管理
        /// </summary>
        /// <returns></returns>
        // GET: OutStorages
        public ActionResult ListOutStorage()
        {
            return View();
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}