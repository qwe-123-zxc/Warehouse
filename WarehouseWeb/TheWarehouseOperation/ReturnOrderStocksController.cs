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

        //查询明细
        public ActionResult QueryMinXi(string id)
        {
            Expression<Func<ReturnOrderStock, bool>> where = i => i.ReturnNum.IndexOf(id) != -1;
            var s = returnOrderStock.GetByWhere(where).SingleOrDefault();
            var d = returnOrderdetail.GetByWhere(i => i.ReturnId.IndexOf(id) != -1);
            var t = returnOrderType.GetByWhere(i => i.Id == s.ReturnTypeId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                ReturnNum = s.ReturnNum,
                ReturnTypeId = t.ReturnTypeName,
                Status = s.Status,
                Num = s.Num,
                AuditUser = s.AuditUser,
                AuditTime = s.AuditTime.ToString("yyyy-MM-dd"),
                Remark = s.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, ReturnId = i.ReturnId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, Sum = i.Sum, Location = i.Location});
            var result = new
            {
                ReturnOrderStockInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //修改审核状态
        public ActionResult UpdtStatus(ReturnOrderStock i, string status)
        {
            var ss = returnOrderStock.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.ReturnNum = ss.ReturnNum;
            i.ReturnTypeId = ss.ReturnTypeId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var returnOrderStocks = new ReturnOrderStockManager();
            var s = returnOrderStocks.Update(i);
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