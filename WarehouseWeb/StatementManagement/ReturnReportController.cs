﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.StatementManagement
{
    /// <summary>
    /// 退货报表页面
    /// </summary>
    public class ReturnReportController : Controller
    {
        // GET: Return
        public ActionResult Index()
        {
            return View();
        }
    }
}