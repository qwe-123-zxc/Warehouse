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

        //查询明细
        public ActionResult QueryMinXi(string id)
        {
            Expression<Func<BadReport, bool>> where = i => i.BadNum.IndexOf(id) != -1;
            var b = badReport.GetByWhere(where).SingleOrDefault();
            var d = badReportDetail.GetByWhere(i => i.BadId.IndexOf(id) != -1);
            var t = badReportType.GetByWhere(i => i.Id == b.BadTypeId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = b.Id,
                BadNum = b.BadNum,
                BadTypeId = t.BadTypeName,
                Status = b.Status,
                Num = b.Num,
                AuditUser = b.AuditUser,
                AuditTime = b.AuditTime.ToString("yyyy-MM-dd"),
                Remark = b.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, BadId = i.BadId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, Quantity = i.Quantity, Location = i.Location });
            var result = new
            {
                BadReportInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(BadReport i, string status)
        {
            var ss = badReport.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.BadNum = ss.BadNum;
            i.BadTypeId = ss.BadTypeId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.SumMoney = ss.SumMoney;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var badReports = new BadReportManager();
            var s = badReports.Update(i);
            var result = new
            {
                ActionResult = s
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}