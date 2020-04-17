using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;
using System.Linq.Expressions;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class BadReportsController : Controller
    {
        BadReportManager badReport = new BadReportManager();
        BadReportDetailManager badReportDetail = new BadReportDetailManager();
        BadReportTypeManager badReportType = new BadReportTypeManager();
        /// <summary>
        /// 报损管理
        /// </summary>
        /// <returns></returns>
        // GET: BadReports
        public ActionResult ListBadReport()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string BadNum, string state, string end,int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<BadReport, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(BadNum))
            {
                where = where.And(i => i.BadNum.IndexOf(BadNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var s = badReport.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, BadNum = i.BadNum, BadTypeId = i.BadTypeId, Num = i.Num, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                BadReportInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}