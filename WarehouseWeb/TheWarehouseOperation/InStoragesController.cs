using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;
using System.Linq.Expressions;
using WarehouseWeb.Models.Dto;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class InStoragesController : QueryBaseControllers
    {
        /// <summary>
        /// 入库管理
        /// </summary>
        /// <returns></returns>
        // GET: InStorages
        public ActionResult List(QueryInStorageDto dto)
        {
            Expression<Func<InStorage, bool>> where = item => item.AuditTime >= dto.State && item.AuditTime <= dto.End;
            if (!string.IsNullOrEmpty(dto.InSNum))
            {
                where = where.And(i => i.InSNum.IndexOf(dto.InSNum) != -1);
            }
            if (dto.InSTypeId!=9999)
            {
                where = where.And(i => i.InSTypeId == dto.InSTypeId);
            }
            if (dto.SupplierId != 99999999)
            {
                where = where.And(i => i.SupplierId == dto.SupplierId);
            }

            if (!string.IsNullOrEmpty(dto.dsh))
            {
                where = where.And(i => i.Status == dto.dsh);
            }

            if (!string.IsNullOrEmpty(dto.shtg))
            {
                where = where.And(i => i.Status == dto.shtg);
            }

            if (!string.IsNullOrEmpty(dto.shsb))
            {
                where = where.And(i => i.Status == dto.shsb);
            }
            //查询所有
            GetRequestPage(dto);
            var pageIndex = dto.PageIndex;
            var count = 0;
            var pageCount = 0;
            var listInfo = new InStorageManager();
            var info = listInfo.GetByWhereDesc(where,i=>i.AuditTime,ref pageIndex,ref count,ref pageCount,2);

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

            ViewBag.PageIndex = pageIndex;
            ViewBag.Count = count;
            ViewBag.PageCount = pageCount;
            ViewBag.lisInfo = info;
            ViewBag.dto1 = dto;
            return View();
        }
        
        public ActionResult ListAdd()
        {
            return View();
        }
    }
}