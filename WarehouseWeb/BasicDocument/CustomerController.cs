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
            get { return 5; }
        }
        CustomerManager service = new CustomerManager();
        public ActionResult Query(string CustomerNum, int pageIndex) {
            Expression<Func<Customer, bool>> where = item => item.IsDelete==0;
            if (!string.IsNullOrEmpty(CustomerNum))
            {
                where = where.And(item => item.CustomerNum.IndexOf(CustomerNum)!=-1 || item.CustomerName.IndexOf(CustomerNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);

            var newFormatList = list.Select(item => new { Id = item.Id, CustomerNum = item.CustomerNum, CustomerName = item.CustomerName, Contacts = item.Contacts, Phone = item.Phone, Email=item.Email, CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),Fax=item.Fax });

            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(string customerName,string fax,string contacts,string email,String phone,string address,String remark)
        {
            Customer customer = new Customer();
            //获取最大编号
            string CustomerNum = service.GetByWhere(item => item.IsDelete == 0).OrderByDescending(item => item.CustomerNum).Take(1).Select(item => item.CustomerNum).FirstOrDefault();
            if (!string.IsNullOrEmpty(CustomerNum))
            {
                customer.CustomerNum = "000001";
            }
            else { 
                customer.CustomerNum = "00000" + (int.Parse(CustomerNum) + 1);
                int num = int.Parse(CustomerNum);
                if (num >= 9)
                {
                    customer.CustomerNum = "0000" + (int.Parse(CustomerNum) + 1);
                }
                else if(num>=99)
                {
                    customer.CustomerNum = "000" + (int.Parse(CustomerNum) + 1);
                }
            }
            customer.CustomerName = customerName;
            customer.Fax = fax;
            customer.Contacts = contacts;
            customer.Email = email;
            customer.Phone = phone;
            customer.Address = address;
            customer.Remark = remark;
            customer.IsDelete = 0;
            customer.CreateTime = DateTime.Now;
            customer.CreateUser = "DA_0000";
            bool val = service.Add(customer);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int customerNum)
        {
            Customer customer = service.GetByWhere(i => i.Id == customerNum).SingleOrDefault();
            customer.IsDelete = 1;
            bool val = service.Update(customer);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }

        //全选单选删除
        public ActionResult DeleteOther(List<Customer> list)
        {
            bool val = true;
            foreach (var item in list)
            {
                Customer customer = service.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
                customer.IsDelete = 1;
                val = service.Update(customer);
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

        public ActionResult QueryById(string customerNum) {
            Customer customer = service.GetByWhere(item => item.CustomerNum.IndexOf(customerNum)!=-1).SingleOrDefault();
            return Json(customer,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string customerName, string fax, string contacts, string email, String phone, string address, String remark,string customerNum)
        {
            Customer customer = service.GetByWhere(item => item.CustomerNum.IndexOf(customerNum) != -1).SingleOrDefault();
            customer.CustomerName = customerName;
            customer.Fax = fax;
            customer.Contacts = contacts;
            customer.Email = email;
            customer.Phone = phone;
            customer.Address = address;
            customer.Remark = remark;
            bool val = service.Update(customer);
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