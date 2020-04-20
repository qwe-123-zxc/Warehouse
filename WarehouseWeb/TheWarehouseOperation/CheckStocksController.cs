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
    public class CheckStocksController : Controller
    {
        CheckStockManager checkStock = new CheckStockManager();
        CheckStockDetailManager checkStockDetail = new CheckStockDetailManager();
        CheckStockTypeManager checkStockType = new CheckStockTypeManager();
        /// <summary>
        /// 盘点管理
        /// </summary>
        /// <returns></returns>
        // GET: CheckStocks
        public ActionResult ListCheckStock()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string CheckNum, string state, string end, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<CheckStock, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(CheckNum))
            {
                where = where.And(i => i.CheckNum.IndexOf(CheckNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var s = checkStock.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, CheckNum = i.CheckNum, CheckTypeId = i.CheckTypeId, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                CheckStockInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //查询明细
        public ActionResult QueryMinXi(string id)
        {
            Expression<Func<CheckStock, bool>> where = i => i.CheckNum.IndexOf(id) != -1;
            var s = checkStock.GetByWhere(where).SingleOrDefault();
            var d = checkStockDetail.GetByWhere(i => i.CheckId.IndexOf(id) != -1);
            var t = checkStockType.GetByWhere(i => i.Id == s.CheckTypeId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                CheckNum = s.CheckNum,
                CheckTypeId = t.MoveTypeName,
                Status = s.Status,
                AuditUser = s.AuditUser,
                AuditTime = s.AuditTime.ToString("yyyy-MM-dd"),
                Remark = s.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, CheckId = i.CheckId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, Type = i.Type, Measure = i.Measure, UnitPrice = i.UnitPrice, Sum =i.Sum });
            var result = new
            {
                CheckStockInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(CheckStock i, string status)
        {
            var ss = checkStock.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.CheckNum = ss.CheckNum;
            i.CheckTypeId = ss.CheckTypeId;
            i.DetailNum = ss.DetailNum;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var checkStocks = new CheckStockManager();
            var s = checkStocks.Update(i);
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