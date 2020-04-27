using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using WarehouseBLL;
using System.Linq.Expressions;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class CheckStocksController : Controller
    {
        CheckStockManager checkStock = new CheckStockManager();
        CheckStockDetailManager checkStockDetail = new CheckStockDetailManager();
        CheckStockTypeManager checkStockType = new CheckStockTypeManager();
        ProductManager product = new ProductManager();
        AdminManager admin = new AdminManager();
        /// <summary>
        /// 盘点管理
        /// </summary>
        /// <returns></returns>
        // GET: CheckStocks
        public ActionResult ListCheckStock()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string CheckNum, string state, string end, int pageIndex, string UserName)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<CheckStock, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate && i.IsDelete == 0;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(CheckNum))
            {
                where = where.And(i => i.CheckNum.IndexOf(CheckNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var s = checkStock.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            var adm = admin.GetByWhere(i => i.UserName == UserName).SingleOrDefault();
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, CheckNum = i.CheckNum, CheckTypeId = i.CheckStockType.MoveTypeName, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd"), audit = adm.RealName });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                CheckStockInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //查询明细
        public ActionResult QueryMinXi(int id)
        {
            Expression<Func<CheckStock, bool>> where = i => i.Id == id;
            var s = checkStock.GetByWhere(where).SingleOrDefault();
            var d = checkStockDetail.GetByWhere(i => i.CheckId == s.CheckNum);
            var t = checkStockType.GetByWhere(i => i.Id == s.CheckTypeId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                CheckNum = s.CheckNum,
                CheckTypeId = t.MoveTypeName,
                Status = s.Status,
                AuditUser = s.AuditUser,
                AuditTime = s.AuditTime.ToString("yyyy-MM-dd"),
                Remark = s.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, CheckId = i.CheckId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, Type = i.Type, Measure = i.Measure, UnitPrice = i.UnitPrice, Sum =i.Sum });
            var result = new
            {
                CheckStockInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(CheckStock i, string status)
        {
            var ss = checkStock.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.CheckNum = ss.CheckNum;
            i.CheckTypeId = ss.CheckTypeId;
            i.DetailNum = ss.DetailNum;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var checkStocks = new CheckStockManager();
            var s = checkStocks.Update(i);
            var result = new
            {
                ActionResult = s
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            //盘点类型
            var type = checkStockType.GetAll();
            type.Insert(0, new CheckStockType() { Id = 9999, MoveTypeName = "请选择盘点类型" });
            ViewBag.CheckTypeId = new SelectList(type, "Id", "MoveTypeName");
            //产品
            var product_1 = product.GetAll();
            product_1.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product_1, "Id", "ProductName");
            return View();
        }

        public ActionResult QueryByProductId(int Id)
        {
            var productInfo = product.GetByWhere(i => i.Id == Id);
            var newFormatList = productInfo.Select(item => new { Id = item.Id, ProductNum = item.ProductNum, ProductName = item.ProductName, Size = item.Size, PCateId = item.PCateId, MeasureId = item.MeasureId, OutPrice = item.OutPrice, StockNum = item.StockNum });
            return Json(newFormatList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(List<CheckStockDetail> detail, int CheckTypeId, string Remark, string AuditUser)
        {
            string detailNum = "";
            //获取明细表最大编号
            string detailNumBig = checkStockDetail.GetByWhere(i => true).OrderByDescending(i => i.DetailNum).Take(1).Select(i => i.DetailNum).FirstOrDefault();
            if (detailNumBig == null)
            {
                detailNum = "000001";
            }
            else
            {
                detailNum = "00000" + (int.Parse(detailNumBig) + 1);
                int num_1 = int.Parse(detailNumBig);
                if (num_1 >= 9)
                {
                    detailNum = "0000" + (int.Parse(detailNumBig) + 1);
                }
                if (num_1 >= 99)
                {
                    detailNum = "000" + (int.Parse(detailNumBig) + 1);
                }
            }

            string checkNum = "";
            //获取盘点表最大编号
            string checkNumBig = checkStock.GetByWhere(i => true).OrderByDescending(i => i.CheckNum).Take(1).Select(i => i.CheckNum).FirstOrDefault();
            if (checkNumBig == null)
            {
                checkNum = "000001";
            }
            else
            {
                checkNum = "00000" + (int.Parse(checkNumBig) + 1);
                int num_2 = int.Parse(checkNumBig);
                if (num_2 >= 9)
                {
                    checkNum = "0000" + (int.Parse(checkNumBig) + 1);
                }
                if (num_2 >= 99)
                {
                    checkNum = "000" + (int.Parse(checkNumBig) + 1);
                }
            }

            bool val = true;
            string msg = "";
            foreach (var item in detail)
            {
                item.CreateTime = DateTime.Now;
                item.DetailNum = detailNum;
                item.CheckId = checkNum;
                val = checkStockDetail.Add(item);
            }
            if (val)
            {
                CheckStock ost = new CheckStock();
                ost.CheckNum = checkNum;
                ost.CheckTypeId = CheckTypeId;
                ost.DetailNum = detailNum;
                ost.Status = "待审核";
                ost.AuditTime = DateTime.Now;
                ost.AuditUser = AuditUser;
                ost.Remark = Remark;
                ost.IsDelete = 0;
                bool val1 = checkStock.Add(ost);
                if (val1)
                {
                    msg = "新增成功";
                }
                else
                {
                    msg = "新增失败";
                }
                msg = "新增成功";
            }
            else
            {
                msg = "新增失败";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdtList(int id)
        {
            Expression<Func<CheckStock, bool>> where = i => i.Id == id;
            var s = checkStock.GetByWhere(where).SingleOrDefault();
            //盘点类型
            var type = checkStockType.GetAll();
            type.Insert(0, new CheckStockType() { Id = 9999, MoveTypeName = "请选择报损类型" });
            ViewBag.CheckTypeId = new SelectList(type, "Id", "MoveTypeName",s.CheckTypeId);
            //产品
            var product_1 = product.GetAll();
            product_1.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product_1, "Id", "ProductName");
            return View(s);
        }

        public ActionResult QueryByIdMinXiInfo(int id)
        {
            CheckStock ins = checkStock.GetByWhere(i => i.Id == id).SingleOrDefault();
            var mx = checkStockDetail.GetByWhere(i => i.CheckId == ins.CheckNum && i.IsDelete == 0);
            return Json(mx, JsonRequestBehavior.AllowGet);
        }

        //修改盘点单
        public ActionResult UpdtInfo(List<CheckStockDetail> detail, int CheckTypeId, string Remark, string checkNum)
        {
            //先删除明细
            bool val_1 = true;
            var checkStockDetails = new CheckStockDetailManager();
            var mx = checkStockDetails.GetByWhere(i => i.CheckId == checkNum);
            foreach (var item in mx)
            {
                val_1 = checkStockDetails.Delete(item);
            }

            //获取明细表最大编号
            string detailNumBig = checkStockDetail.GetByWhere(item => true).OrderByDescending(item => item.DetailNum).Take(1).Select(item => item.DetailNum).FirstOrDefault();
            string detailNum = "";
            if (detailNumBig == null)
            {
                detailNumBig = "000001";
            }
            else
            {
                detailNum = "00000" + (int.Parse(detailNumBig) + 1);
                int num1 = int.Parse(detailNumBig);
                if (num1 >= 9)
                {
                    detailNumBig = "0000" + (int.Parse(detailNumBig) + 1);
                }
                else if (num1 >= 99)
                {
                    detailNumBig = "000" + (int.Parse(detailNumBig) + 1);
                }
            }
            string msg = "";
            bool val = true;
            foreach (var item in detail)
            {
                item.DetailNum = detailNum;
                item.CreateTime = DateTime.Now;
                item.CheckId = checkNum;
                val = checkStockDetail.Add(item);
            }
            if (val)
            {
                var checkStock_1 = new CheckStockManager();
                var s = checkStock_1.GetByWhere(i => i.CheckNum == checkNum).SingleOrDefault();
                s.DetailNum = detailNum;
                s.CheckTypeId = CheckTypeId;
                s.Remark = Remark;
                bool vall = checkStock.Update(s);
                if (vall)
                {
                    msg = "修改成功";
                }
                else
                {
                    msg = "修改失败";
                }
                msg = "修改成功";
            }
            else
            {
                msg = "修改失败";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        
        //删除盘点单
        public ActionResult DeleteInfo(int id)
        {
            CheckStock ins = checkStock.GetByWhere(item => item.Id == id).SingleOrDefault();
            List<CheckStockDetail> listDetail = checkStockDetail.GetByWhere(item => item.CheckId == ins.CheckNum);
            bool val = true;
            string msg = "";
            foreach (var list in listDetail)
            {
                list.IsDelete = 1;
                val = checkStockDetail.Update(list);
            }
            if (val)
            {
                ins.IsDelete = 1;
                bool vall = checkStock.Update(ins);
                if (vall)
                {
                    msg = "删除成功";
                }
                else
                {
                    msg = "删除失败";
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        //全选单选删除
        public ActionResult DeleteOther(List<CheckStock> list)
        {
            string msg = "";
            foreach (var item in list)
            {
                CheckStock ins = checkStock.GetByWhere(i => i.Id == item.Id).SingleOrDefault();
                List<CheckStockDetail> listDetail = checkStockDetail.GetByWhere(i => i.CheckId == ins.CheckNum);
                bool val = true;
                foreach (var listd in listDetail)
                {
                    listd.IsDelete = 1;
                    val = checkStockDetail.Update(listd);
                }
                if (val)
                {
                    ins.IsDelete = 1;
                    bool vall = checkStock.Update(ins);
                    if (vall)
                    {
                        msg = "删除成功";
                    }
                    else
                    {
                        msg = "删除失败";
                    }
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}