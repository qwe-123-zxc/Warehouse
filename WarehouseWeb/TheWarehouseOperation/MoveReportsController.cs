﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;
using System.Linq.Expressions;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class MoveReportsController : Controller
    {
        MoveReportManager moveReport = new MoveReportManager();
        MoveReportDetailManager moveReportDetail = new MoveReportDetailManager();
        MoveReportTypeManager moveReportType = new MoveReportTypeManager();
        /// <summary>
        /// 移库管理
        /// </summary>
        /// <returns></returns>
        // GET: MoveReports
        public ActionResult ListMoveReport()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string MoveNum, string state, string end, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<MoveReport, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(MoveNum))
            {
                where = where.And(i => i.MoveNum.IndexOf(MoveNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var s = moveReport.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, MoveNum = i.MoveNum, MoveTypeId = i.MoveTypeId, Num = i.Num, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                MoveReportInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}