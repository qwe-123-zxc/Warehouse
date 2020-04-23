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
    /// 产品管理页面
    /// </summary>
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {

            List<ProductCategory> productCategory = categoryManager.GetAll();
            productCategory.Insert(0, new ProductCategory() { Id = 99999999, PCateName = "请选择类别" });
            ViewBag.PCateId = new SelectList(productCategory, "Id", "PCateName");

            List<Measure> measures = measureManager.GetAll();
            measures.Insert(0, new Measure() { Id = 99999999, MeasureName = "请选择单位" });
            ViewBag.MeasuresId = new SelectList(measures, "Id", "MeasureName");

            List<Location> location = locationManager.GetAll();
            location.Insert(0, new Location() { Id = 99999999, LocationName = "请选择单位" });
            ViewBag.LocationId = new SelectList(location, "Id", "LocationName");
            return View();
        }
        ProductCategoryManager categoryManager = new ProductCategoryManager();
        ProductManager productManager = new ProductManager();
        MeasureManage measureManager = new MeasureManage();
        LocationManager locationManager = new LocationManager();
        public int PageSize
        {
            get { return 5; }
        }
        public ActionResult Query(string Name,int PcateId, int pageIndex)
        {
            Expression<Func<Product, bool>> where = item => item.IsDelete == 0;
            if (!string.IsNullOrEmpty(Name))
            {
                where = where.And(item => item.ProductName.IndexOf(Name) != -1);
            }
            if (PcateId!=99999999)
            {
                where = where.And(item => item.PCateId==PcateId);
            }
            var pageCount = 0;
            var count = 0;
            var list = productManager.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);

            var newFormatList = list.Select(item => new { Id = item.Id, ProductNum = item.ProductNum, ProductName = item.ProductName, MaxNum = item.MaxNum, MinNum = item.MinNum, OutPrice = item.OutPrice, Size = item.Size, PCateId = item.ProductCategory.PCateName, MeasureId = item.Measure.MeasureName, Remark = item.Remark });

            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Insert(string ProductName,int MaxNum,int MinNum,double OutPrice, string Size, string Color,int PCateId,int MeasureId,int LocationId)
        {
            Product product = new Product();
            //编号
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            date = date.Replace("-", "");
            date = date.Replace(" ", "");
            date = date.Replace(":", "");
            product.ProductNum = date;
            product.ProductName = ProductName;
            product.MaxNum = MaxNum;
            product.MinNum = MinNum;
            product.OutPrice = OutPrice;
            product.Size = Size;
            product.Color = Color;
            product.PCateId = PCateId;
            product.MeasureId = MeasureId;
            product.LocationId = LocationId;
            product.IsDelete = 0;
            product.CreateTime = DateTime.Now;
            product.CreateUser = "DA_0000";
            bool val = productManager.Add(product);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        public ActionResult Delete(int measureNum)
        {
            Measure measure = service.GetByWhere(item => item.Id == measureNum).SingleOrDefault();
            measure.IsDelete = 1;
            bool val = service.Update(measure);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult DeleteOther(List<Measure> list)
        //{
        //    bool val = true;
        //    foreach (var item in list)
        //    {
        //        Measure measure = service.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
        //        measure.IsDelete = 1;
        //        val = service.Update(measure);
        //    }
        //    if (val)
        //    {
        //        return Json("删除成功", JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json("删除失败", JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult QueryById(int measureNum)
        //{
        //    Measure measure = service.GetByWhere(item => item.Id == measureNum).SingleOrDefault();
        //    return Json(measure, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult Update(string measureName, int measureNum)
        //{
        //    Measure measure = service.GetByWhere(item => item.Id == measureNum).SingleOrDefault();
        //    measure.MeasureName = measureName;
        //    bool val = service.Update(measure);
        //    if (val)
        //    {
        //        return Json("修改成功", JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json("修改失败", JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}