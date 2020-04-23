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

        //查询明细
        public ActionResult QueryMinXi(string id)
        {
            Expression<Func<MoveReport, bool>> where = i => i.MoveNum.IndexOf(id) != -1;
            var s = moveReport.GetByWhere(where).SingleOrDefault();
            var d = moveReportDetail.GetByWhere(i => i.MoveId.IndexOf(id) != -1);
            var t = moveReportType.GetByWhere(i => i.Id == s.MoveTypeId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                MoveNum = s.MoveNum,
                MoveTypeId = t.MoveTypeName,
                Status = s.Status,
                Num = s.Num,
                AuditUser = s.AuditUser,
                AuditTime = s.AuditTime.ToString("yyyy-MM-dd"),
                Remark = s.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, MoveId = i.MoveId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, Quantity = i.Quantity, TheCurrentLocation = i.TheCurrentLocation, MovingLocation = i.MovingLocation });
            var result = new
            {
                MoveReportInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(MoveReport i, string status)
        {
            var ss = moveReport.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.MoveNum = ss.MoveNum;
            i.MoveTypeId = ss.MoveTypeId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var moveReports = new MoveReportManager();
            var s = moveReports.Update(i);
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