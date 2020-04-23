using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;
using System.Linq.Expressions;
namespace WarehouseWeb.BasicDocument
{
    /// <summary>
    /// 供应商管理页面
    /// </summary>
    public class SupplierController : Controller
    {
        // GET: Supplier
        public ActionResult Index()
        {
            var s = SupplierTypeManager.GetAll();
            s.Insert(0, new SupplierType() { Id = 9999, SupplierTypeName = "请选择供应商" });
            ViewBag.SupTypeId = new SelectList(s, "Id", "SupplierTypeName");
            return View();
        }
        public int PageSize
        {
            get { return 5; }
        }
        SupplierManager service = new SupplierManager();
        SupplierTypeManager SupplierTypeManager = new SupplierTypeManager();
        public ActionResult Query(string SupplierNum,int pageIndex) {
            Expression<Func<Supplier, bool>> where = item => true;
            if (!string.IsNullOrEmpty(SupplierNum))
            {
                where = where.And(item => item.SupplierNum.IndexOf(SupplierNum) != -1 || item.SupplierNum.IndexOf(SupplierNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);

            var newFormatList = list.Select(item => new { Id = item.Id, SupplierNum = item.SupplierNum, SupplierName = item.SupplierName, SupTypeId = item.SupplierType.SupplierTypeName, Phone=item.Phone,Fax=item.Fax,Email=item.Email,Contacts=item.Contacts,Address=item.Address,Describe=item.Describe });

            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(string supplierNum)
        {
            Supplier supplier = service.GetByWhere(item => item.SupplierNum.IndexOf(supplierNum) != -1).SingleOrDefault();
            bool val = service.Delete(supplier);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult Add() {
        //    var xlk = SupplierTypeManager.GetAll();
        //    xlk.Insert(0, new SupplierType() { Id = 9999, SupplierTypeName = "请选择入库单类型" });
        //    ViewBag.InSTypeId = new SelectList(xlk, "Id", "SupplierTypeName");
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Insert(string supplierName, int supTypeId, string phone, string contacts, string email, string fax, string address, string describe)
        {
            Supplier supplier = new Supplier();
            //获取最大编号
            string SupplierNum = service.GetByWhere(item => true).OrderByDescending(item => item.SupplierNum).Take(1).Select(item => item.SupplierNum).FirstOrDefault();
            supplier.SupplierNum = "00000" + (int.Parse(SupplierNum) + 1);
            int num = int.Parse(SupplierNum);
            if (num >= 9)
            {
                supplier.SupplierNum = "0000" + (int.Parse(SupplierNum) + 1);
            }
            else if (num >= 99)
            {
                supplier.SupplierNum = "000" + (int.Parse(SupplierNum) + 1);
            }
            supplier.SupplierName = supplierName;
            supplier.SupTypeId = supTypeId;
            supplier.Phone = phone;
            supplier.Contacts = contacts;
            supplier.Email = email;
            supplier.Fax = fax;
            supplier.Address = address;
            supplier.Describe = describe;
            supplier.IsDelete = 0;
            supplier.CreateTime = DateTime.Now;
            supplier.CreateUser = "DA_0000";
            bool val = service.Add(supplier);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult QueryById(string supplierNum)
        {
            Supplier supplier = service.GetByWhere(item => item.SupplierNum.IndexOf(supplierNum) != -1).SingleOrDefault();
            return Json(supplier, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string supplierName, string supplierNum, int supTypeId, string phone, string contacts, string email, string fax, string address, string describe)
        {
            Supplier supplier = service.GetByWhere(item => item.SupplierNum.IndexOf(supplierNum) != -1).SingleOrDefault();
            supplier.SupplierName = supplierName;
            supplier.SupTypeId = supTypeId;
            supplier.Phone = phone;
            supplier.Contacts = contacts;
            supplier.Email = email;
            supplier.Fax = fax;
            supplier.Address = address;
            supplier.Describe = describe;
            bool val = service.Update(supplier);
            if (val)
            {
                return Json("修改成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("修改失败", JsonRequestBehavior.AllowGet);
            }
        }
    }
}