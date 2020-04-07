using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.StatementManagement
{
    /// <summary>
    /// 台账记录页面
    /// </summary>
    public class StandingBookController : Controller
    {
        // GET: StandingBook
        public ActionResult Index()
        {
            return View();
        }
    }
}