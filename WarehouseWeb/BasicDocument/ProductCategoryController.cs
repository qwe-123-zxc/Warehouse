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
            Expression<Func<ProductCategory, bool>> where = item => true;
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
            string PCateNum = service.GetByWhere(item => true).OrderByDescending(item => item.PCateNum).Take(1).Select(item => item.PCateNum).FirstOrDefault();
            productCategory.PCateNum = "00000" + (int.Parse(PCateNum) + 1);
            int num = int.Parse(PCateNum);
            if (num >= 9)
            {
                productCategory.PCateNum = "0000" + (int.Parse(PCateNum) + 1);
            }
            else if (num>=99) {
                productCategory.PCateNum = "000" + (int.Parse(PCateNum) + 1);
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
        public ActionResult Delete(string pcateNum)
        {
            ProductCategory productCategory = service.GetByWhere(item => item.PCateNum.IndexOf(pcateNum) != -1).SingleOrDefault();
            bool val = service.Delete(productCategory);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult QueryById(string pcateNum)
        {
            ProductCategory productCategory = service.GetByWhere(item => item.PCateNum.IndexOf(pcateNum) != -1).SingleOrDefault();
            return Json(productCategory, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string pcateName, string pcateNum)
        {
            ProductCategory productCategory = service.GetByWhere(item => item.PCateNum.IndexOf(pcateNum) != -1).SingleOrDefault();
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