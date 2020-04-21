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
        Adminmanager adminManger = new Adminmanager();
        RolePowerManager rolePowerManager = new RolePowerManager();

        public ActionResult UserAndPwd(string userName, string pwd)
        {
            Admin admin = adminManger.GetByWhere(item => item.UserName == userName && item.Password == pwd).SingleOrDefault();
            if (admin != null)
            {
                var result = new
                {
                    user = admin,
                    msg= "登陆成功"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("登陆失败", JsonRequestBehavior.AllowGet);
            }
        }

        //侧边栏
        public ActionResult GetSidebarData(string userName, string pwd)
        {
            Admin admin = adminManger.GetByWhere(item => item.UserName == userName && item.Password == pwd).SingleOrDefault();
            if (pwd==null)
            {
                admin = adminManger.GetByWhere(item => item.UserName == userName).SingleOrDefault();
            }
            this.HttpContext.Session["userName"] = admin.UserName ;
            if (admin != null)
            {
                List<Function> list = functionManager.GetByLinq(admin.RoleId);
                List<Function> rootMeun = list.Where(item => item.ParentNodeId == 0 && item.IsDelete == 0).ToList();
                var newList = new
                {
                    list = list,
                    rootMeun = rootMeun,
                    msg="登陆成功"
                };
                return Json(newList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("登陆失败", JsonRequestBehavior.AllowGet);
            }
        }
    }
}