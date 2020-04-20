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
        SupplierManager GonYinShang = new SupplierManager();    //供应商
        InStorageTypeManager inStorageType = new InStorageTypeManager();    //入库类型
        InStorageManager inStorage = new InStorageManager();    //入库管理
        InStorageDetailManager inStorageDetail = new InStorageDetailManager(); //入库明细
        /// <summary>
        /// 入库管理
        /// </summary>
        /// <returns></returns>
        // GET: InStorages
        public ActionResult List()
        {
            //供应商
            var gys = GonYinShang.GetAll();
            gys.Insert(0, new Supplier() { Id = 99999999, SupplierName = "请选择供应商" });
            ViewBag.SupplierId = new SelectList(gys, "Id", "SupplierName");
            //单据类型
            var lty = inStorageType.GetAll();
            lty.Insert(0, new InStorageType() { Id = 9999, InSTypeName = "请选择入库单类型" });
            ViewBag.InSTypeId = new SelectList(lty, "Id", "InSTypeName");

            return View();
        }

        public ActionResult ListAjax(string zt, string InSNum, string state, string end, int SupplierId, int InSTypeId, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
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
            var newFormatList = s.Select(i => new { id=i.Id, InSNum = i.InSNum, InSTypeId = i.InSTypeId, SupplierId = i.SupplierId, Num = i.Num, SumMoney = i.SumMoney, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                InstorageInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //查询明细
        public ActionResult QueryMinXi(int id)
        {
            Expression<Func<InStorage, bool>> where = i => i.Id==id;
            var s = inStorage.GetByWhere(where).SingleOrDefault();
            var d = inStorageDetail.GetByWhere(i => i.InStorageId == s.Id);
            var t = inStorageType.GetByWhere(i => i.Id == s.InSTypeId).SingleOrDefault();
            var g = GonYinShang.GetByWhere(i => i.Id == s.SupplierId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                rukudanhao = s.InSNum,
                rukuleixin = t.InSTypeName,
                zt = s.Status,
                gonyinshangId = g.SupplierNum,
                gonyinshangName = g.SupplierName,
                lianxiren = g.Contacts,
                chuanjianren = s.AuditUser,
                chuanjianTime = s.AuditTime.ToString("yyyy-MM-dd"),
                phone = g.Phone,
                beizhu = s.Remark
            };
            //明细
            var dd = d.Select(i=>new { Id=i.Id, DetailNum=i.DetailNum, InStorageId=i.InStorageId, ProductNum=i.ProductNum, ProductName=i.ProductName, Size=i.Size, UnitPrice=i.UnitPrice, Quantity=i.Quantity, SumMoney=i.SumMoney, Location=i.Location });
            var result = new
            {
                InstorageInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(InStorage i,string status)
        {
            var ss = inStorage.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.InSNum = ss.InSNum;
            i.InSTypeId = ss.InSTypeId;
            i.SupplierId = ss.SupplierId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.SumMoney = ss.SumMoney;
            i.Status = status;
            i.Operation = ss.Operation;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var inStorages = new InStorageManager();
            var s = inStorages.Update(i);
            var result = new
            {
                ActionResult = s
            };
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            return View();
        }
    }
}