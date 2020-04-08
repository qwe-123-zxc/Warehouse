using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class InStoragesController : Controller
    {
        /// <summary>
        /// 入库管理
        /// </summary>
        /// <returns></returns>
        // GET: InStorages
        public ActionResult List()
        {
            return View();
        }
        
        public ActionResult ListAdd()
        {
            return View();
        }
    }
}