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
    public class ReturnOrderStocksController : Controller
    {
        ReturnOrderStockManager returnOrderStock = new ReturnOrderStockManager();
        ReturnOrderDetailManager returnOrderdetail = new ReturnOrderDetailManager();
        ReturnOrderTypeManager returnOrderType = new ReturnOrderTypeManager();
        /// <summary>
        /// 退货管理
        /// </summary>
        /// <returns></returns>
        // GET: ReturnOrderStocks
        public ActionResult ListReturnOrderStock()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string ReturnNum, string state, string end, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<ReturnOrderStock, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(ReturnNum))
            {
                where = where.And(i => i.ReturnNum.IndexOf(ReturnNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var s = returnOrderStock.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, ReturnNum = i.ReturnNum, ReturnTypeId = i.ReturnTypeId, Num = i.Num, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                ReturnOrderStockInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}