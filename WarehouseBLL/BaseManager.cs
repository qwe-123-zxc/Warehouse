using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using WarehouseDAL;
using System.Linq.Expressions;

namespace WarehouseBLL
{
    /// <summary>
    /// 业务逻辑基类
    /// </summary>
    public class BaseManager<T> where T : class
    {
        BaseService<T> baseService = null;
        public BaseService<T> MyService
        {
            get
            {
                if (baseService == null)
                {
                    baseService = new BaseService<T>();
                }
                return baseService;
            }
        }
        /// <summary>
        /// 获取所有商品信息
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll()
        {
            return MyService.GetAll();
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetByWhere(Expression<Func<T, bool>> where)
        {
            return MyService.GetByWhere(where);
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetByWhereAsc<orderByT>(Expression<Func<T, bool>> where, Expression<Func<T, orderByT>> orderBy, ref int pageIndex, ref int count, ref int pageCount, int pageSize)
        {
            return MyService.GetByWhereAsc(where, orderBy, ref pageIndex, ref count, ref pageCount, pageSize);
        }

        /// <summary>
        /// 条件降序查询 带分页 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> GetByWhereDesc<orderByT>(Expression<Func<T, bool>> where, Expression<Func<T, orderByT>> orderBy, ref int pageIndex, ref int count, ref int pageCount, int pageSize)
        {
            return MyService.GetByWhereDesc(where, orderBy, ref pageIndex, ref count, ref pageCount, pageSize);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(T model)
        {
            return MyService.Add(model);
        }

        /// <summary>
        /// 修改
        /// 修改的实体 必须主键有值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(T model)
        {
            return MyService.Update(model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Delete(T model)
        {
            return MyService.Delete(model);
        }
    }
}
