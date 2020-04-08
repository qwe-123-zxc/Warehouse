using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}