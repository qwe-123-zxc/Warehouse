using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Linq.Expressions;
using WarehouseBLL;
namespace WarehouseWeb.BasicDocument
{
    /// <summary>
    /// 计量单位页面
    /// </summary>
    public class MeasureController : Controller
    {
        // GET: Measure
        public ActionResult Index()
        {
            return View();
        }
        public int PageSize
        {
            get { return 5; }
        }
        MeasureManage service = new MeasureManage();
        public ActionResult Query(string MeasureNum, int pageIndex)
        {
            Expression<Func<Measure, bool>> where = item => item.IsDelete==0;
            if (!string.IsNullOrEmpty(MeasureNum))
            {
                where = where.And(item => item.MeasureNum.IndexOf(MeasureNum) != -1 || item.MeasureName.IndexOf(MeasureNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);

            var newFormatList = list.Select(item => new { Id = item.Id, MeasureNum = item.MeasureNum, MeasureName = item.MeasureName,  CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });

            var result = new
            {
                PageIndex = pageIndex,
                PageCount = pageCount,
                Count = count,
                RoleInfies = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Insert(string measureName)
        {
            Measure measure = new Measure();
            //获取最大编号
            string MeasureNum = service.GetByWhere(item => true).OrderByDescending(item => item.MeasureNum).Take(1).Select(item => item.MeasureNum).FirstOrDefault();
            measure.MeasureNum = "00000" + (int.Parse(MeasureNum) + 1);
            int num = int.Parse(MeasureNum);
            if (num >= 9)
            {
                measure.MeasureNum = "0000" + (int.Parse(MeasureNum) + 1);
            }
            else if (num >= 99)
            {
                measure.MeasureNum = "000" + (int.Parse(MeasureNum) + 1);
            }
            measure.MeasureName = measureName;
            measure.IsDelete = 0;
            measure.CreateTime = DateTime.Now;
            measure.CreateUser = "DA_0000";
            bool val = service.Add(measure);
            if (val)
            {
                return Json("新增成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("新增失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(int measureNum)
        {
            Measure measure = service.GetByWhere(item => item.Id==measureNum).SingleOrDefault();
            measure.IsDelete = 1;
            bool val = service.Update(measure);
            if (val)
            {
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("删除失败", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteOther(List<Measure> list)
        {
            bool val = true;
            foreach (var item in list)
            {
                Measure measure = service.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
                measure.IsDelete = 1;
                val = service.Update(measure);
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
        public ActionResult QueryById(int measureNum)
        {
            Measure measure = service.GetByWhere(item => item.Id==measureNum).SingleOrDefault();
            return Json(measure, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(string measureName,int measureNum)
        {
            Measure measure = service.GetByWhere(item => item.Id==measureNum).SingleOrDefault();
            measure.MeasureName = measureName;
            bool val = service.Update(measure);
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