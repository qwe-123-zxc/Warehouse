using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.StatementManagement
{
    /// <summary>
    /// 入库报表页面
    /// </summary>
    public class InStorageController : Controller
    {
        // GET: InStorage
        public ActionResult Index()
        {
            return View();
        }
    }
}