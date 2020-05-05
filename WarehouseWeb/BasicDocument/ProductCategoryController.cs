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
    /// 产品类别页面
    /// </summary>
    public class ProductCategoryController : Controller
    {
        // GET: ProductCategory
        public ActionResult Index()
        {
            return View();
        }
        public int PageSize
        {
            get { return 5; }
        }
        ProductCategoryManager service = new ProductCategoryManager();
        public ActionResult Query(string PCateNum, int pageIndex)
        {
            Expression<Func<ProductCategory, bool>> where = item => item.IsDelete==0;
            if (!string.IsNullOrEmpty(PCateNum))
            {
                where = where.And(item => item.PCateNum.IndexOf(PCateNum) != -1 || item.PCateName.IndexOf(PCateNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);

            var newFormatList = list.Select(item => new { Id = item.Id, PCateNum = item.PCateNum, PCateName = item.PCateName, CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });

            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Insert(string pcateName)
        {
            ProductCategory productCategory = new ProductCategory();
            //获取最大编号
            string PCateNum = service.GetByWhere(item => item.IsDelete == 0).OrderByDescending(item => item.PCateNum).Take(1).Select(item => item.PCateNum).FirstOrDefault();
            if (string.IsNullOrEmpty(PCateNum))
            {
                productCategory.PCateNum = "000001";
            }
            else
            {
                productCategory.PCateNum = "00000" + (int.Parse(PCateNum) + 1);
                int num = int.Parse(PCateNum);
                if (num >= 9)
                {
                    productCategory.PCateNum = "0000" + (int.Parse(PCateNum) + 1);
                }
                else if (num >= 99)
                {
                    productCategory.PCateNum = "000" + (int.Parse(PCateNum) + 1);
                }
            }
            productCategory.PCateName = pcateName;
            productCategory.IsDelete = 0;
            productCategory.CreateTime = DateTime.Now;
            productCategory.CreateUser = "DA_0000";
            bool val = service.Add(productCategory);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(int pcateNum)
        {
            ProductCategory productCategory = service.GetByWhere(item => item.Id == pcateNum).SingleOrDefault();
            productCategory.IsDelete = 1;
            bool val = service.Update(productCategory);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteOther(List<ProductCategory> list)
        {
            bool val = true;
            foreach (var item in list)
            {
                ProductCategory productCategory = service.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
                productCategory.IsDelete = 1;
                val = service.Update(productCategory);
            }
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult QueryById(int pcateNum)
        {
            ProductCategory productCategory = service.GetByWhere(item => item.Id==pcateNum).SingleOrDefault();
            return Json(productCategory, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string pcateName, int pcateNum)
        {
            ProductCategory productCategory = service.GetByWhere(item => item.Id==pcateNum&&item.IsDelete==0).SingleOrDefault();
            productCategory.PCateName = pcateName;
            bool val = service.Update(productCategory);
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