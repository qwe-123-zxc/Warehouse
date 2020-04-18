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
    public class OutStoragesController : Controller
    {
        OutStorageManager outStorage = new OutStorageManager();
        OutStorageTypeManager outStorageType = new OutStorageTypeManager();
        OutStorageDetailManager outStorageDetail = new OutStorageDetailManager();
        CustomerManager customer = new CustomerManager();

        /// <summary>
        /// 出库管理
        /// </summary>
        /// <returns></returns>
        // GET: OutStorages
        public ActionResult ListOutStorage()
        {
            //客户
            var gys = customer.GetAll();
            gys.Insert(0, new Customer() { Id = 99999999, CustomerName = "请选择供应商" });
            ViewBag.CustomerId = new SelectList(gys, "Id", "CustomerName");
            //单据类型
            var lty = outStorageType.GetAll();
            lty.Insert(0, new OutStorageType() { Id = 9999, OutSTypeName = "请选择出库单类型" });
            ViewBag.OutSTypeId = new SelectList(lty, "Id", "OutSTypeName");
            return View();
        }

        public ActionResult ListAjax(string zt, string OutSNum, string state, string end, int CustomerId, int OutSTypeId, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<OutStorage, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(OutSNum))
            {
                where = where.And(i => i.OutSNum.IndexOf(OutSNum) != -1);
            }
            if (CustomerId != 99999999)
            {
                where = where.And(i => i.CustomerId == CustomerId);
            }
            if (OutSTypeId != 9999)
            {
                where = where.And(i => i.OutSTypeId == OutSTypeId);
            }
            var pageCount = 0;
            var count = 0;
            var s = outStorage.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, OutSNum = i.OutSNum, OutSTypeId = i.OutSTypeId, CustomerId = i.CustomerId, Num = i.Num, SumMoney = i.SumMoney, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                OutstorageInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}