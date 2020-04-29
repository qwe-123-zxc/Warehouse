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
    /// 库位管理页面
    /// </summary>
    public class LocationController : Controller
    { 
        /// <summary>
       /// 每页显示2条
      /// </summary>
        public int PageSize
        {
            get { return 2; }
        }
        // GET: Location
        public ActionResult Index()
        {
            StorageManager StorageManager = new StorageManager();
            var Storage = StorageManager.GetAll();
            Storage.Insert(0, new Storage() { Id = 0, StorageName = "请选择" });
            ViewBag.StorageId = new SelectList(Storage, "Id", "StorageName");


            LocationTypeManager LocationTypeManager = new LocationTypeManager();
            var LocationType = LocationTypeManager.GetAll();
            LocationType.Insert(0, new LocationType() { Id = 0, LocaTypeName = "请选择" });
            ViewBag.LocationTypeId = new SelectList(LocationType, "Id", "LocaTypeName");

            return View();
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLocation(int StorageId, int LocaTypeId, string LocationName, int pageIndex)
        {

            LocationManager service = new LocationManager();

            //组合条件
            Expression<Func<Location, bool>> where = item=>true;
            
            if (!string.IsNullOrEmpty(LocationName))
            {
                //库位名条件查询
                where = where.And(item => item.LocationName == LocationName);
            }
            if (LocaTypeId != 0)
            {
                //库位类型条件查询
                where = where.And(item => item.LocaTypeId == LocaTypeId);
            }
            if (StorageId != 0)
            {
                //仓库编号条件查询
                where = where.And(item => item.StorageId == StorageId);
            }
           


            var pageCount = 0;
            var count = 0;
            var list = service.GetByWhereDesc(where, item => item.CreateTime, ref pageIndex, ref count, ref pageCount, PageSize);
            //返回数据
            //Actionresult  常用响应类型  ViewResult ContentResult JsonResult
            // Json数据格式 { 名称:值 } 数组 [{},{}]
            // 格式转换
            var newFormatList = list.Select(item => new { Id = item.Id, LocationNum = item.LocationNum, LocationName = item.LocationName, StorageId = item.Storage.StorageName, LocaTypeId = item.LocationType.LocaTypeName, IsDelete = item.IsDelete, IsDefault = item.IsDefault,CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") });

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


        LocationManager LocationManager = new LocationManager();

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        public ActionResult AddAjax(string LocationName, int StorageId, int LocationTypeId,string LocationNum)
        {
            Location location = new Location();
            //获取库位最大编号
            string locationNum = LocationManager.GetByWhere(item => item.IsDelete == 0).OrderByDescending(item => item.LocationNum).Take(1).Select(item => item.LocationNum).FirstOrDefault();
            if (!string.IsNullOrEmpty(locationNum))
            {
                location.LocationNum = "000001";
            }
            else
            {
                location.LocationNum = "00000" + (int.Parse(locationNum) + 1);
                int num = int.Parse(locationNum);
                if (num >= 9)
                {
                    location.LocationNum = "0000" + (int.Parse(LocationNum) + 1);
                }
                else if (num >= 99)
                {
                    location.LocationNum = "000" + (int.Parse(LocationNum) + 1);
                }
            }
            location.LocationName = LocationName;
            location.StorageId = StorageId;
            location.LocaTypeId = LocationTypeId;
            location.IsDelete = 0;
            location.CreateTime = DateTime.Now;
            bool val = LocationManager.Add(location);
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
        /// <param name="LocationId"></param>
        /// <returns></returns>
        public ActionResult QueryByLocationId(int LocationId)
        {
            Location location = LocationManager.GetByWhere(item => item.Id == LocationId).SingleOrDefault();
            return Json(location, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="USerId"></param>
        /// <returns></returns>
        public ActionResult Update(string LocationNum, string LocationName, string locationName,int StorageId,int storageId, int LocationTypeId, int locaTypeId)
        {
            Location location = LocationManager.GetByWhere(item => item.LocationNum == LocationNum).SingleOrDefault();
            location.LocationName = LocationName;
            location.StorageId = storageId;
            location.LocaTypeId = LocationTypeId;
            location.IsDelete = 0;
            location.CreateTime = DateTime.Now;
            bool val = LocationManager.Update(location);
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
        public ActionResult Delete(int locationId)
        {
            Location location = LocationManager.GetByWhere(item => item.Id == locationId).SingleOrDefault();
            location.IsDelete = 1;
            bool val = LocationManager.Update(location);
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