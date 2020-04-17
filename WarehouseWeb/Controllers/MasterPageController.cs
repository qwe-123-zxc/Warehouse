using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;

namespace WarehouseWeb.Controllers
{
    public class MasterPageController : Controller
    {
        // GET: MasterPage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }

        FunctionManager functionManager = new FunctionManager();
        AdminManger adminManger = new AdminManger();
        RolePowerManager rolePowerManager = new RolePowerManager();
        RoleManeger roleManeger = new RoleManeger();
        

        //侧边栏——判断用户是否存在
        public ActionResult GetSidebarData(string userName, string pwd)
        {
            Admin admin = adminManger.GetByWhere(item => item.UserName == userName && item.Password == pwd).SingleOrDefault();
            if (pwd==null)
            {
                admin = adminManger.GetByWhere(item => item.UserName == userName).SingleOrDefault();
            }
            if (admin != null)
            {
                List<Function> list = functionManager.GetByLinqRoleId(admin.RoleId);
                List<Function> rootMeun = list.Where(item => item.ParentNodeId == 0 && item.IsDelete == 0).ToList();
                var newList = new
                {
                    list = list,
                    rootMeun = rootMeun,
                    msg="登陆成功"
                };
                this.HttpContext.Session["userName"] = admin.UserName;
                return Json(newList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("登陆失败", JsonRequestBehavior.AllowGet);
            }
        }

        //查询个人信息
        public ActionResult QueryPersonal(string username)
        {
            Admin admin = adminManger.GetByWhere(item => item.UserName == username).SingleOrDefault();
            Role roles = roleManeger.GetByWhere(item => item.Id == admin.RoleId).SingleOrDefault();
            var newAdmin = new
            {
               userName = admin.UserName,
               userCode = admin.UserCode,
               realName = admin.RealName,
               email = admin.Email,
               phone=admin.Phone,
               role=roles.RoleName
            };
            var result = new
            {
                adminData = newAdmin
            };
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        //修改个人信息
        public ActionResult UpdatePersonal(Admin admin,string userName)
        {
            Admin admins = adminManger.GetByWhere(item => item.UserName == userName ).SingleOrDefault();
            admins.Phone = admin.Phone;
            admins.RealName = admin.RealName;
            admins.Email = admin.Email;
            bool val = adminManger.Update(admins);
            if (val)
            {
                return Json("修改成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("修改失败", JsonRequestBehavior.AllowGet);
            }
        }

        //修改密码
        public ActionResult UpdatePwd(string passWord, string pwd, string userName)
        {
            Admin admin = adminManger.GetByWhere(item => item.UserName == userName).SingleOrDefault();
            if (admin.Password.Equals(passWord))
            {
                admin.Password = pwd;
                bool val = adminManger.Update(admin);
                if (val)
                {
                    return Json("修改成功", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("修改失败", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("当前密码错误", JsonRequestBehavior.AllowGet);
            }
        }
    }
}