using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.StatementManagement
{
    /// <summary>
    /// 货品统计页面
    /// </summary>
    public class GoodsStatisticsController : Controller
    {
        // GET: GoodsStatistics
        public ActionResult Index()
        {
            return View();
        }
    }
}