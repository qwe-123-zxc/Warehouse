using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;
using System.Linq.Expressions;

namespace WarehouseWeb.SystemSetup
{
    /// <summary>
    /// 菜单管理页面
    /// </summary>
    public class MenuController : Controller
    {
        // GET: Menu
        public ActionResult Index()
        {
            List<Function> list = functionManager.GetByWhere(item=>item.IsDelete==0&&item.ParentNodeId==0);
            list.Insert(0, new Function() { Id = 0, DisplayName = "请选择" });
            ViewBag.list = new SelectList(list, "Id", "DisplayName");
            return View();
        }
        public int PageSize
        {
            get { return 8; }
        }
        FunctionManager functionManager = new FunctionManager();
        public ActionResult Query(string Name, int pageIndex)
        {
            Expression<Func<Function, bool>> where = item => item.IsDelete == 0;
            if (!string.IsNullOrEmpty(Name))
            {
                where = where.And(item => item.DisplayName.IndexOf(Name) != -1 || item.NodeId.Equals(Name));
            }
            var pageCount = 0;
            var count = 0;
            var list = functionManager.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);
            
            var newFormatList = list.Select(item => new { Id = item.Id, DisplayName = item.DisplayName, ParentNodeId = item.ParentNodeId, NodeId = item.NodeId, NodeURL = item.NodeURL, CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });
            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(string DisplayName, int ParentNodeId, string NodeURL)
        {
            Function function = new Function();
            //获取编号
            if (ParentNodeId == 9999999)
            {
                int NodeId = functionManager.GetByWhere(item => item.IsDelete == 0 && item.ParentNodeId == 0).OrderByDescending(item => item.NodeId).Take(1).Select(item => item.NodeId).FirstOrDefault();
                function.NodeId = NodeId+10000;
                function.DisplayName = DisplayName;
                function.ParentNodeId = 0;
                function.CreateTime = DateTime.Now;
                function.CreateUser = "DA_0000";
                function.IsDelete = 0;
                function.NodeURL = NodeURL;
            }
            else
            {
                int NodeId = functionManager.GetByWhere(item => item.IsDelete == 0 && item.ParentNodeId == ParentNodeId).OrderByDescending(item => item.NodeId).Take(1).Select(item => item.NodeId).FirstOrDefault();
                function.NodeId = NodeId+1000;
                function.DisplayName = DisplayName;
                function.ParentNodeId = ParentNodeId;
                function.CreateTime = DateTime.Now;
                function.CreateUser = "DA_0000";
                function.IsDelete = 0;
                function.NodeURL = NodeURL;
            }
            bool val = functionManager.Add(function);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult Delete(int customerNum)
        //{
        //    Customer customer = service.GetByWhere(i => i.Id == customerNum).SingleOrDefault();
        //    customer.IsDelete = 1;
        //    bool val = service.Update(customer);
        //    if (val)
        //    {
        //        return Json("删除成功", JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json("删除失败", JsonRequestBehavior.AllowGet);
        //    }
        //}

        ////全选单选删除
        //public ActionResult DeleteOther(List<Customer> list)
        //{
        //    bool val = true;
        //    foreach (var item in list)
        //    {
        //        Customer customer = service.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
        //        customer.IsDelete = 1;
        //        val = service.Update(customer);
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

        //public ActionResult QueryById(string customerNum)
        //{
        //    Customer customer = service.GetByWhere(item => item.CustomerNum.IndexOf(customerNum) != -1).SingleOrDefault();
        //    return Json(customer, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult Update(string customerName, string fax, string contacts, string email, String phone, string address, String remark, string customerNum)
        //{
        //    Customer customer = service.GetByWhere(item => item.CustomerNum.IndexOf(customerNum) != -1).SingleOrDefault();
        //    customer.CustomerName = customerName;
        //    customer.Fax = fax;
        //    customer.Contacts = contacts;
        //    customer.Email = email;
        //    customer.Phone = phone;
        //    customer.Address = address;
        //    customer.Remark = remark;
        //    bool val = service.Update(customer);
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