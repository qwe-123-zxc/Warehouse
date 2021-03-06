﻿using System;
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
    /// 部门管理页面
    /// </summary>
    public class DepartmentController : Controller
    {
        /// <summary>
        /// 每页显示2条
        /// </summary>
        public int PageSize
        {
            get { return 2; }
        }
        // GET: Department
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDepart(string DepartNum, int pageIndex)
        {

            Departmanager service = new Departmanager();
            //组合条件
            Expression<Func<Depart, bool>> where = item => item.IsDelete==0;

            if (!string.IsNullOrEmpty(DepartNum))
            {
                //当类型不是全部选中项，则按照类型组合条件
                where = where.And(item => item.DepartNum.IndexOf(DepartNum) != -1 || item.DepartName.IndexOf(DepartNum) != -1);
            }


            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);
            //返回数据
            //Actionresult  常用响应类型  ViewResult ContentResult JsonResult
            // Json数据格式 { 名称:值 } 数组 [{},{}]
            // 格式转换
            var newFormatList = list.Select(item => new { Id = item.Id, DepartNum = item.DepartNum, DepartName = item.DepartName, IsDelete = item.IsDelete, CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });

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


        Departmanager Departmanager = new Departmanager();
        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="Depart"></param>
        /// <returns></returns>
        public ActionResult AddAjax(string DepartName)
        {
            Depart depart = new Depart();
            //获取最大编号
            string departNum = Departmanager.GetByWhere(item => item.IsDelete == 0).OrderByDescending(item => item.DepartNum).Take(1).Select(item => item.DepartNum).FirstOrDefault();
            if (string.IsNullOrEmpty(departNum))
            {
                depart.DepartNum = "000001";
            }
            else
            {
                depart.DepartNum = "00000" + (int.Parse(departNum) + 1);
                int num = int.Parse(departNum);
                if (num >= 9)
                {
                    depart.DepartNum = "0000" + (int.Parse(departNum) + 1);
                }
                else if (num >= 99)
                {
                    depart.DepartNum = "000" + (int.Parse(departNum) + 1);
                }
            }
            depart.DepartName = DepartName;
            depart.IsDelete = 0;
            depart.CreateTime = DateTime.Now;
            bool val = Departmanager.Add(depart);
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
        /// <param name="DepartId"></param>
        /// <returns></returns>
        public ActionResult QueryByDepartId(int DepartId)
        {
            Depart depart = Departmanager.GetByWhere(item => item.Id == DepartId).SingleOrDefault();
            return Json(depart, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="DepartId"></param>
        /// <returns></returns>
        public ActionResult Update(string departNum, string DepartName)
        {
            Depart depart = Departmanager.GetByWhere(item => item.DepartNum == departNum && item.IsDelete == 0).SingleOrDefault();
            depart.DepartName = DepartName;

            bool val = Departmanager.Update(depart);
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
        /// <param name="DepartId"></param>
        /// <returns></returns>
        public ActionResult Delete(int departId)
        {
            Depart depart = Departmanager.GetByWhere(item => item.Id == departId).SingleOrDefault();
            depart.IsDelete = 1;
            bool val = Departmanager.Update(depart);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteOther(List<Depart> list)
        {
            bool val = true;
            foreach (var item in list)
            {
                Depart depart = Departmanager.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
                depart.IsDelete = 1;
                val = Departmanager.Update(depart);
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
    }
}
