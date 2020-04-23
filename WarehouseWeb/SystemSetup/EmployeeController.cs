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
    /// 员工管理页面
    /// </summary>
    public class EmployeeController : Controller
    {
        /// <summary>
        /// 每页显示2条
        /// </summary>
        public int PageSize
        {
            get { return 2; }
        }
        // GET: Employee
        public ActionResult Index()
        {
            Departmanager Departmanager = new Departmanager();
            var Depart = Departmanager.GetAll();
            Depart.Insert(0, new Depart() { Id = 0, DepartName = "请选择部门类型" });
            ViewBag.DepartId = new SelectList(Depart, "Id", "DepartName");

            Rolemanager Rolemanager = new Rolemanager();
            var Role = Rolemanager.GetAll();
            Role.Insert(0, new Role() { Id = 0, RoleName = "请选择角色类型" });
            ViewBag.RoleId = new SelectList(Role, "Id", "RoleName");

            return View();
        }
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAdmin(string UserCode, int RoleId, int DepartId, string UserName, string UserCode1, int pageIndex)
        {

            AdminManager service = new AdminManager();
           
            //组合条件
            Expression<Func<Admin, bool>> where = item => item.IsDelete==0;

            if (!string.IsNullOrEmpty(UserCode))
            {
                //条件查询
                where = where.And(item => item.UserCode.IndexOf(UserCode)!=-1 || item.UserName.IndexOf(UserCode) != -1);
            }
            if (RoleId!=0)
            {
                //角色条件查询
                where = where.And(item => item.RoleId==RoleId);
            }
            if (DepartId!=0)
            {
                //部门条件查询
                where = where.And(item => item.DepartId==DepartId);
            }
            if (!string.IsNullOrEmpty(UserCode1))
            {
                //员工编号条件查询
                where = where.And(item => item.UserCode.IndexOf(UserCode1) != -1);
            }
            if (!string.IsNullOrEmpty(UserName))
            {
                //员工名称条件查询
                where = where.And(item => item.UserName.IndexOf(UserName) != -1);
            }


            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);
            //返回数据
            //Actionresult  常用响应类型  ViewResult ContentResult JsonResult
            // Json数据格式 { 名称:值 } 数组 [{},{}]
            // 格式转换
            var newFormatList = list.Select(item => new { Id = item.Id, UserCode = item.UserCode, UserName = item.UserName, Password = item.Password, RealName = item.RealName, Email = item.Email, Phone = item.Phone, LoginCount = item.LoginCount, DepartId = item.DepartId, RoleId = item.RoleId, CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });

            //将数据构建打包给前台
            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        AdminManager Adminmanager = new AdminManager();

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public ActionResult AddAjax(string UserName,string Password, string RealName, string Email, string Phone, int DepartId, int RoleId)
        {
            Admin admin = new Admin();
            //获取最大编号
            string UserCode = Adminmanager.GetByWhere(item => item.Id != 1).OrderByDescending(item => item.UserCode).Take(1).Select(item => item.UserCode).FirstOrDefault();
            admin.UserCode = "00000" + (int.Parse(UserCode) + 1);

            int num = int.Parse(UserCode);
            if (num >= 9)
            {
                admin.UserCode = "0000" + (int.Parse(UserCode) + 1);
            }
            else if (num >= 99)
            {
                admin.UserCode = "000" + (int.Parse(UserCode) + 1);
            }

            admin.UserName = UserName;
            admin.Password = Password;
            admin.RealName = RealName;
            admin.Email = Email;
            admin.Phone = Phone;
            admin.DepartId = DepartId;
            admin.RoleId = RoleId;
            admin.IsDelete = 0;
            admin.CreateTime = DateTime.Now;
            bool val = Adminmanager.Add(admin);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult QueryByAdminId(int UserId)
        {
            Admin admin = Adminmanager.GetByWhere(item => item.Id == UserId).SingleOrDefault();
            return Json(admin, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="USerId"></param>
        /// <returns></returns>
        public ActionResult Update(string UserCode, string userName, string UserName,string RealName, string realName, string Email, string email, string Phone, string phone, int DepartId, int departId, int RoleId, int roleId)
        {
            Admin admin = Adminmanager.GetByWhere(item => item.UserCode == UserCode).SingleOrDefault();
            admin.UserName = UserName;
            admin.RealName = RealName;
            admin.Email = Email;
            admin.Phone = Phone;
            admin.DepartId = DepartId;
            admin.RoleId = RoleId;
            admin.IsDelete = 0;
            admin.CreateTime = DateTime.Now;
            bool val = Adminmanager.Update(admin);
            if (val)
            {
                return Json("修改成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("修改失败", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="USerId"></param>
        /// <returns></returns>
        public ActionResult Delete(int adminId)
        {
            Admin admin = Adminmanager.GetByWhere(item => item.Id == adminId).SingleOrDefault();
            bool val = Adminmanager.Delete(admin);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
    }
}