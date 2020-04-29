using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;

namespace WarehouseWeb.SystemSetup
{
    /// <summary>
    /// 权限分配页面
    /// </summary>
    public class PermissionGrantedController : Controller
    {

        FunctionManager functionManager = new FunctionManager();
        RolePowerManager rolePowerManager = new RolePowerManager();

        
        // GET: PermissionGranted
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowData(int roleId)
        {
            //已分配
            List<Function> list = functionManager.GetByLinqRoleId(roleId);
            List<Function> rootMeun = list.Where(item => item.ParentNodeId == 0 && item.IsDelete == 0).ToList();
            //未分配
            List<Function> rootMeun1 = functionManager.GetByLinqRoleIdNot(roleId);
            //父级编号
            List<Function> listParentNodeId = functionManager.GetByLinqParentNodeId(roleId);

            var result = new
            {
                rightList = list,
                rightListRoot = rootMeun,
                leftListRoot = rootMeun1,
                listParentNodeId = listParentNodeId
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(int RoleId,int NodeId,int ParentNodeId)
        {
            RolePower role = rolePowerManager.GetByWhere(item=>item.RoleId==RoleId&&item.NodeId==ParentNodeId).SingleOrDefault();
            if (role==null)
            {
                RolePower r = new RolePower();
                r.RoleId = RoleId;
                r.NodeId = ParentNodeId;
                r.CreateTime = DateTime.Now;
                r.IsDelete = 0;
                bool vall = rolePowerManager.Add(r);
            }
            RolePower rolePower = new RolePower();
            rolePower.RoleId = RoleId;
            rolePower.NodeId = NodeId;
            rolePower.CreateTime = DateTime.Now;
            rolePower.IsDelete = 0;
            bool val = rolePowerManager.Add(rolePower);
            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(int RoleId, int NodeId, int ParentNodeId)
        {
            RolePower role = rolePowerManager.GetByWhere(item => item.RoleId == RoleId && item.NodeId == ParentNodeId).SingleOrDefault();
            if (role == null)
            {
                RolePower r = new RolePower();
                r.RoleId = RoleId;
                r.NodeId = ParentNodeId;
                r.CreateTime = DateTime.Now;
                r.IsDelete = 0;
                bool vall = rolePowerManager.Add(r);
            }
            RolePower rolePower = rolePowerManager.GetByWhere(item => item.RoleId == RoleId && item.NodeId == NodeId).SingleOrDefault();
            bool val = rolePowerManager.Delete(rolePower);
            return Json(val, JsonRequestBehavior.AllowGet);
        }
    }
}