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
    public class OutStoragesController : Controller
    {
        OutStorageManager outStorage = new OutStorageManager();
        OutStorageTypeManager outStorageType = new OutStorageTypeManager();
        OutStorageDetailManager outStorageDetail = new OutStorageDetailManager();
        CustomerManager customer = new CustomerManager();
        ProductManager productManager = new ProductManager();//产品表
        LocationManager location = new LocationManager();
        AdminManager admin = new AdminManager();
        
        /// <summary>
        /// 出库管理
        /// </summary>
        /// <returns></returns>
        // GET: OutStorages
        public ActionResult ListOutStorage()
        {
            //客户
            var gys = customer.GetAll();
            gys.Insert(0, new Customer() { Id = 99999999, CustomerName = "请选择供应商" });
            ViewBag.CustomerId = new SelectList(gys, "Id", "CustomerName");
            //单据类型
            var lty = outStorageType.GetAll();
            lty.Insert(0, new OutStorageType() { Id = 9999, OutSTypeName = "请选择出库单类型" });
            ViewBag.OutSTypeId = new SelectList(lty, "Id", "OutSTypeName");
            return View();
        }

        public ActionResult ListAjax(string zt, string OutSNum, string state, string end, int CustomerId, int OutSTypeId, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<OutStorage, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate && i.IsDelete == 0;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(OutSNum))
            {
                where = where.And(i => i.OutSNum.IndexOf(OutSNum) != -1);
            }
            if (CustomerId != 99999999)
            {
                where = where.And(i => i.CustomerId == CustomerId);
            }
            if (OutSTypeId != 9999)
            {
                where = where.And(i => i.OutSTypeId == OutSTypeId);
            }
            var pageCount = 0;
            var count = 0;
            var s = outStorage.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, OutSNum = i.OutSNum, OutSTypeId = i.OutStorageType.OutSTypeName, CustomerId = i.Customer.CustomerName, Num = i.Num, SumMoney = i.SumMoney, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                OutstorageInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //查询明细
        public ActionResult QueryMinXi(string id)
        {
            Expression<Func<OutStorage, bool>> where = i => i.OutSNum.IndexOf(id) != -1;
            var s = outStorage.GetByWhere(where).SingleOrDefault();
            var d = outStorageDetail.GetByWhere(i => i.OutStorageId.IndexOf(id) != -1);
            var t = outStorageType.GetByWhere(i => i.Id == s.OutSTypeId).SingleOrDefault();
            var k = customer.GetByWhere(i => i.Id == s.CustomerId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                OutSNum = s.OutSNum,
                OutSTypeId = t.OutSTypeName,
                Status = s.Status,
                CustomerNum = k.CustomerNum,
                CustomerName = k.CustomerName,
                Num = s.Num,
                AuditUser = s.AuditUser,
                AuditTime = s.AuditTime.ToString("yyyy-MM-dd"),
                Remark = s.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, OutStorageId = i.OutStorageId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, UnitPrice = i.UnitPrice, Quantity = i.Quantity, SumMoney = i.SumMoney, Location = i.Location });
            var result = new
            {
                outStorageInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //修改审核状态
        public ActionResult UpdtStatus(OutStorage i, string status)
        {
            var ss = outStorage.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.OutSNum = ss.OutSNum;
            i.OutSTypeId = ss.OutSTypeId;
            i.CustomerId = ss.CustomerId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.SumMoney = ss.SumMoney;
            i.Status = status;
            i.Contacts = ss.Contacts;
            i.SendDate = ss.SendDate;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var outStorages = new OutStorageManager();
            var s = outStorages.Update(i);
            var result = new
            {
                ActionResult = s
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            //出库单类型
            var outtype = outStorageType.GetAll();
            outtype.Insert(0, new OutStorageType() { Id = 9999, OutSTypeName = "请选择出库单类型" });
            ViewBag.OutSTypeId = new SelectList(outtype, "Id", "OutSTypeName");
            //客户
            var cust = customer.GetAll();
            cust.Insert(0, new Customer() { Id = 9999, CustomerName = "请选择客户" });
            ViewBag.CustomerId = new SelectList(cust, "Id", "CustomerName");
            //产品
            var product = productManager.GetAll();
            product.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product, "Id", "ProductName");
            return View();
        }

        public ActionResult QueryByProductId(int Id)
        {
            var productInfo = productManager.GetByWhere(i => i.Id == Id);
            var newFormatList = productInfo.Select(item => new { Id = item.Id, ProductNum = item.ProductNum, ProductName = item.ProductName, Size = item.Size, OutPrice = item.OutPrice, LocationId = item.Location.LocationName, StockNum = item.StockNum });
            return Json(productInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(List<OutStorageDetail> detail, int OutSTypeId, int CustomerId, string Remark, string AuditUser)
        {
            string detailNum = "";
            //获取明细表最大编号
            string detailNumBig = outStorageDetail.GetByWhere(i => true).OrderByDescending(i => i.DetailNum).Take(1).Select(i => i.DetailNum).FirstOrDefault();
            if (detailNumBig==null)
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

            string outSNum = "";
            //获取出库表最大编号
            string outSNumBig = outStorage.GetByWhere(i => true).OrderByDescending(i => i.OutSNum).Take(1).Select(i => i.OutSNum).FirstOrDefault();
            if (outSNumBig==null)
            {
                outSNum = "000001";
            }
            else
            {
                outSNum = "00000" + (int.Parse(outSNumBig) + 1);
                int num_2 = int.Parse(outSNumBig);
                if (num_2 >= 9)
                {
                    outSNum = "0000" + (int.Parse(outSNumBig) + 1);
                }
                if (num_2 >= 99)
                {
                    outSNum = "000" + (int.Parse(outSNumBig) + 1);
                }
            }

            bool val = true;
            string msg = "";
            foreach (var item in detail)
            {
                item.CreateTime = DateTime.Now;
                item.DetailNum = detailNum;
                item.OutStorageId = outSNum;
                val = outStorageDetail.Add(item);
            }
            if (val)
            {
                var num = outStorageDetail.GetByWhere(i => i.OutStorageId == outSNum).Sum(i => i.Quantity); //获取总出货数
                var sumMoney = outStorageDetail.GetByWhere(i => i.OutStorageId == outSNum).Sum(i => i.SumMoney); //获取总价格
                OutStorage ost = new OutStorage();
                ost.OutSNum = outSNum;
                ost.OutSTypeId = OutSTypeId;
                ost.CustomerId = CustomerId;
                ost.DetailNum = detailNum;
                ost.Num = Convert.ToInt32(num); ;
                ost.SumMoney = Convert.ToDouble(sumMoney);
                ost.Status = "待审核";
                ost.AuditTime = DateTime.Now;
                ost.AuditUser = AuditUser;
                ost.Remark = Remark;
                ost.IsDelete = 0;
                bool val1 = outStorage.Add(ost);
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
            Expression<Func<OutStorage, bool>> where = i => i.Id == id;
            var s = outStorage.GetByWhere(where).SingleOrDefault();
            //出库单类型
            var outtype = outStorageType.GetAll();
            outtype.Insert(0, new OutStorageType() { Id = 9999, OutSTypeName = "请选择出库单类型" });
            ViewBag.OutSTypeId = new SelectList(outtype, "Id", "OutSTypeName",s.OutSTypeId);
            //客户
            var cust = customer.GetAll();
            cust.Insert(0, new Customer() { Id = 9999, CustomerName = "请选择客户" });
            ViewBag.CustomerId = new SelectList(cust, "Id", "CustomerName",s.CustomerId);
            //产品
            var product = productManager.GetAll();
            product.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product, "Id", "ProductName");
            return View(s);
        }

        public ActionResult QueryByIdMinXiInfo(int id)
        {
            OutStorage ins = outStorage.GetByWhere(i => i.Id == id).SingleOrDefault();
            var mx = outStorageDetail.GetByWhere(i => i.OutStorageId == ins.OutSNum && i.IsDelete == 0);
            return Json(mx, JsonRequestBehavior.AllowGet);
        }

        //修改出库单
        public ActionResult UpdtInfo(List<OutStorageDetail> detail, int outSTypeId, int customerId, string Remark, string OutSNum)
        {
            //先删除明细
            bool val_1 = true;
            var outStorageDetails = new OutStorageDetailManager();
            var mx = outStorageDetails.GetByWhere(i => i.OutStorageId == OutSNum);
            foreach (var item in mx)
            {
                val_1 = outStorageDetails.Delete(item);
            }

            //获取明细表最大编号
            string detailNumBig = outStorageDetail.GetByWhere(item => true).OrderByDescending(item => item.DetailNum).Take(1).Select(item => item.DetailNum).FirstOrDefault();
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
                item.OutStorageId = OutSNum;
                item.CreateTime = DateTime.Now;
                val = outStorageDetail.Add(item);
            }
            if (val)
            {
                var num = outStorageDetail.GetByWhere(item => item.OutStorageId == OutSNum).Sum(item => item.Quantity);
                var sumMoney = outStorageDetail.GetByWhere(item => item.OutStorageId == OutSNum).Sum(item => item.SumMoney);
                var outStorage_1 = new OutStorageManager();
                var s = outStorage_1.GetByWhere(i => i.OutSNum == OutSNum).SingleOrDefault();
                s.DetailNum = detailNum;
                s.OutSTypeId = outSTypeId;
                s.CustomerId = customerId;
                s.Remark = Remark;
                s.Num = Convert.ToInt32(num);
                s.SumMoney = Convert.ToDouble(sumMoney);
                bool vall = outStorage.Update(s);
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

        //删除出库单
        public ActionResult DeleteInfo(int id)
        {
            OutStorage ins = outStorage.GetByWhere(item => item.Id == id).SingleOrDefault();
            List<OutStorageDetail> listDetail = outStorageDetail.GetByWhere(item => item.OutStorageId == ins.OutSNum);
            bool val = true;
            string msg = "";
            foreach (var list in listDetail)
            {
                list.IsDelete = 1;
                val = outStorageDetail.Update(list);
            }
            if (val)
            {
                ins.IsDelete = 1;
                bool vall = outStorage.Update(ins);
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
    }
}