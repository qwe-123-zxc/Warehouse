using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.BasicDocument
{
    /// <summary>
    /// 供应商管理页面
    /// </summary>
    public class SupplierController : Controller
    {
        // GET: Supplier
        public ActionResult Index()
        {
            return View();
        }
    }
}