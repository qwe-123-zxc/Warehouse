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

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}