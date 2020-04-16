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
    public class InStoragesController : System.Web.Mvc.Controller
    {
        /// <summary>
        /// 入库管理
        /// </summary>
        /// <returns></returns>
        // GET: InStorages
        public ActionResult List()
        {
            //供应商
            var GonYinShang = new SupplierManager();
            var gys = GonYinShang.GetAll();
            gys.Insert(0, new Supplier() { Id = 99999999, SupplierName = "请选择供应商" });
            ViewBag.SupplierId = new SelectList(gys, "Id", "SupplierName");
            //单据类型
            var listType = new InStorageTypeManager();
            var lty = listType.GetAll();
            lty.Insert(0, new InStorageType() { Id = 9999, InSTypeName = "请选择入库单类型" });
            ViewBag.InSTypeId = new SelectList(lty, "Id", "InSTypeName");

            return View();
        }

        public ActionResult ListAjax(string zt, string InSNum, string state, string end, int SupplierId, int InSTypeId, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            var inStorage = new InStorageManager();
            Expression<Func<InStorage, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(InSNum))
            {
                where = where.And(i => i.InSNum.IndexOf(InSNum) != -1);
            }
            if (SupplierId != 99999999)
            {
                where = where.And(i => i.SupplierId == SupplierId);
            }
            if (InSTypeId != 9999)
            {
                where = where.And(i => i.InSTypeId == InSTypeId);
            }
            var pageCount = 0;
            var count = 0;
            var s = inStorage.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { InSNum = i.InSNum, InSTypeId = i.InSTypeId, SupplierId = i.SupplierId, Num = i.Num, SumMoney = i.SumMoney, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                InstorageInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}