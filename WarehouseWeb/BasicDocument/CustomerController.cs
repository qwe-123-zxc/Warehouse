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
    /// 客户管理页面
    /// </summary>
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        public int PageSize
        {
            get { return 2; }
        }
        public ActionResult Query(string CustomerNum, int pageIndex) {
            CustomerManager service = new CustomerManager();
            Expression<Func<Customer, bool>> where = item => true;
            if (!string.IsNullOrEmpty(CustomerNum))
            {
                where = where.And(item => item.CustomerNum.IndexOf(CustomerNum)!=-1 || item.CustomerName.IndexOf(CustomerNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);

            var newFormatList = list.Select(item => new { Id = item.Id, CustomerNum = item.CustomerNum, CustomerName = item.CustomerName, Contacts = item.Contacts, Phone = item.Phone, Email=item.Email, CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });

            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}