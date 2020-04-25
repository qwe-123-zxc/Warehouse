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
    public class ReturnOrderStocksController : Controller
    {
        ReturnOrderStockManager returnOrderStock = new ReturnOrderStockManager();
        ReturnOrderDetailManager returnOrderdetail = new ReturnOrderDetailManager();
        ReturnOrderTypeManager returnOrderType = new ReturnOrderTypeManager();
        ProductManager product = new ProductManager();
        CustomerManager customer = new CustomerManager();
        /// <summary>
        /// 退货管理
        /// </summary>
        /// <returns></returns>
        // GET: ReturnOrderStocks
        public ActionResult ListReturnOrderStock()
        {
            return View();
        }

        public ActionResult ListAjax(string zt, string ReturnNum, string state, string end, int pageIndex)
        {
            var stateDate = Convert.ToDateTime(state);
            var endDate = Convert.ToDateTime(end);
            Expression<Func<ReturnOrderStock, bool>> where = i => i.AuditTime >= stateDate && i.AuditTime <= endDate && i.IsDelete == 0;
            if (!string.IsNullOrEmpty(zt))
            {
                where = where.And(i => i.Status == zt);
            }
            if (!string.IsNullOrEmpty(ReturnNum))
            {
                where = where.And(i => i.ReturnNum.IndexOf(ReturnNum) != -1);
            }
            var pageCount = 0;
            var count = 0;
            var s = returnOrderStock.GetByWhereDesc(where, item => item.AuditTime, ref pageIndex, ref count, ref pageCount, 2);
            //格式转换
            var newFormatList = s.Select(i => new { id = i.Id, ReturnNum = i.ReturnNum, ReturnTypeId = i.ReturnOrderType.ReturnTypeName, Num = i.Num, Status = i.Status, AuditUser = i.AuditUser, AuditTime = i.AuditTime.ToString("yyyy-MM-dd") });
            var result = new
            {
                PageCount = pageCount,
                Count = count,
                PageIndex = pageIndex,
                ReturnOrderStockInfo = newFormatList
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //查询明细
        public ActionResult QueryMinXi(int id)
        {
            Expression<Func<ReturnOrderStock, bool>> where = i => i.Id == id;
            var s = returnOrderStock.GetByWhere(where).SingleOrDefault();
            var d = returnOrderdetail.GetByWhere(i => i.ReturnId == s.ReturnNum);
            var t = returnOrderType.GetByWhere(i => i.Id == s.ReturnTypeId).SingleOrDefault();
            //主表显示
            var info = new
            {
                id = s.Id,
                ReturnNum = s.ReturnNum,
                ReturnTypeId = t.ReturnTypeName,
                Status = s.Status,
                Num = s.Num,
                AuditUser = s.AuditUser,
                AuditTime = s.AuditTime.ToString("yyyy-MM-dd"),
                Remark = s.Remark
            };
            //明细
            var dd = d.Select(i => new { Id = i.Id, DetailNum = i.DetailNum, ReturnId = i.ReturnId, ProductNum = i.ProductNum, ProductName = i.ProductName, Size = i.Size, Sum = i.Sum, Location = i.Location});
            var result = new
            {
                ReturnOrderStockInfo = info,
                XiangXiInfo = dd
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //修改审核状态
        public ActionResult UpdtStatus(ReturnOrderStock i, string status)
        {
            var ss = returnOrderStock.GetByWhere(item => item.Id == i.Id).SingleOrDefault();
            i.ReturnNum = ss.ReturnNum;
            i.ReturnTypeId = ss.ReturnTypeId;
            i.DetailNum = ss.DetailNum;
            i.Num = ss.Num;
            i.Status = status;
            i.AuditUser = ss.AuditUser;
            i.AuditTime = ss.AuditTime;
            i.IsDelete = ss.IsDelete;
            i.Remark = ss.Remark;
            var returnOrderStocks = new ReturnOrderStockManager();
            var s = returnOrderStocks.Update(i);
            var result = new
            {
                ActionResult = s
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAdd()
        {
            //退货类型
            var type = returnOrderType.GetAll();
            type.Insert(0, new ReturnOrderType() { Id = 9999, ReturnTypeName = "请选择退货类型" });
            ViewBag.ReturnTypeId = new SelectList(type, "Id", "ReturnTypeName");
            //产品
            var product_1 = product.GetAll();
            product_1.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product_1, "Id", "ProductName");
            //客户
            var customer_1 = customer.GetAll();
            customer_1.Insert(0, new Customer() { Id = 9999, CustomerName = "请选择客户" });
            ViewBag.CustomerId = new SelectList(customer_1, "Id", "CustomerName");
            return View();
        }
        
        public ActionResult QueryByProductId(int Id)
        {
            var productInfo = product.GetByWhere(i => i.Id == Id);
            var newFormatList = productInfo.Select(item => new { Id = item.Id, ProductNum = item.ProductNum, ProductName = item.ProductName, Size = item.Size, LocationId = item.LocationId, StockNum = item.StockNum });
            return Json(newFormatList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Insert(List<ReturnOrderDetail> detail, int ReturnTypeId, int CustomerId, string Remark, string AuditUser)
        {
            string detailNum = "";
            //获取明细表最大编号
            string detailNumBig = returnOrderdetail.GetByWhere(i => true).OrderByDescending(i => i.DetailNum).Take(1).Select(i => i.DetailNum).FirstOrDefault();
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

            string returnNum = "";
            //获取退货表最大编号
            string returnNumBig = returnOrderStock.GetByWhere(i => true).OrderByDescending(i => i.ReturnNum).Take(1).Select(i => i.ReturnNum).FirstOrDefault();
            if (returnNumBig == null)
            {
                returnNum = "000001";
            }
            else
            {
                returnNum = "00000" + (int.Parse(returnNumBig) + 1);
                int num_2 = int.Parse(returnNumBig);
                if (num_2 >= 9)
                {
                    returnNum = "0000" + (int.Parse(returnNumBig) + 1);
                }
                if (num_2 >= 99)
                {
                    returnNum = "000" + (int.Parse(returnNumBig) + 1);
                }
            }

            bool val = true;
            string msg = "";
            foreach (var item in detail)
            {
                item.CreateTime = DateTime.Now;
                item.DetailNum = detailNum;
                item.ReturnId = returnNum;
                val = returnOrderdetail.Add(item);
            }
            if (val)
            {
                var num = returnOrderdetail.GetByWhere(i => i.ReturnId == returnNum).Sum(i => i.Sum); //获取总出货数
                ReturnOrderStock ost = new ReturnOrderStock();
                ost.ReturnNum = returnNum;
                ost.ReturnTypeId = ReturnTypeId;
                ost.CustomerId = CustomerId;
                ost.DetailNum = detailNum;
                ost.Num = Convert.ToInt32(num);
                ost.Status = "待审核";
                ost.AuditTime = DateTime.Now;
                ost.AuditUser = AuditUser;
                ost.Remark = Remark;
                ost.IsDelete = 0;
                bool val1 = returnOrderStock.Add(ost);
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
            Expression<Func<ReturnOrderStock, bool>> where = i => i.Id == id;
            var s = returnOrderStock.GetByWhere(where).SingleOrDefault();
            //退货类型
            var type = returnOrderType.GetAll();
            type.Insert(0, new ReturnOrderType() { Id = 9999, ReturnTypeName = "请选择退货类型" });
            ViewBag.ReturnTypeId = new SelectList(type, "Id", "ReturnTypeName",s.ReturnTypeId);
            //产品
            var product_1 = product.GetAll();
            product_1.Insert(0, new Product() { Id = 9999, ProductName = "请选择产品" });
            ViewBag.Product = new SelectList(product_1, "Id", "ProductName");
            //客户
            var customer_1 = customer.GetAll();
            customer_1.Insert(0, new Customer() { Id = 9999, CustomerName = "请选择客户" });
            ViewBag.CustomerId = new SelectList(customer_1, "Id", "CustomerName",s.CustomerId);
            return View(s);
        }

        public ActionResult QueryByIdMinXiInfo(int id)
        {
            ReturnOrderStock ins = returnOrderStock.GetByWhere(i => i.Id == id).SingleOrDefault();
            var mx = returnOrderdetail.GetByWhere(i => i.ReturnId == ins.ReturnNum && i.IsDelete == 0);
            return Json(mx, JsonRequestBehavior.AllowGet);
        }

        //修改退货单
        public ActionResult UpdtInfo(List<ReturnOrderDetail> detail, int ReturnTypeId, int CustomerId, string Remark, string ReturnNum)
        {
            //先删除明细
            bool val_1 = true;
            var returnOrderDetails = new ReturnOrderDetailManager();
            var mx = returnOrderDetails.GetByWhere(i => i.ReturnId == ReturnNum);
            foreach (var item in mx)
            {
                val_1 = returnOrderDetails.Delete(item);
            }

            //获取明细表最大编号
            string detailNumBig = returnOrderdetail.GetByWhere(item => true).OrderByDescending(item => item.DetailNum).Take(1).Select(item => item.DetailNum).FirstOrDefault();
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
                item.CreateTime = DateTime.Now;
                item.DetailNum = detailNum;
                item.ReturnId = ReturnNum;
                val = returnOrderdetail.Add(item);
            }
            if (val)
            {
                var num = returnOrderdetail.GetByWhere(item => item.ReturnId == ReturnNum).Sum(item => item.Sum);
                var returnOrderStock_1 = new ReturnOrderStockManager();
                var s = returnOrderStock_1.GetByWhere(i => i.ReturnNum == ReturnNum).SingleOrDefault();
                s.DetailNum = detailNum;
                s.ReturnTypeId = ReturnTypeId;
                s.CustomerId = CustomerId;
                s.Remark = Remark;
                s.Num = Convert.ToInt32(num);
                bool vall = returnOrderStock.Update(s);
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
            ReturnOrderStock ins = returnOrderStock.GetByWhere(item => item.Id == id).SingleOrDefault();
            List<ReturnOrderDetail> listDetail = returnOrderdetail.GetByWhere(item => item.ReturnId == ins.ReturnNum);
            bool val = true;
            string msg = "";
            foreach (var list in listDetail)
            {
                list.IsDelete = 1;
                val = returnOrderdetail.Update(list);
            }
            if (val)
            {
                ins.IsDelete = 1;
                bool vall = returnOrderStock.Update(ins);
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