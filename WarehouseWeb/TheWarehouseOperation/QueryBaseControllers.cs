using WarehouseWeb.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseWeb.TheWarehouseOperation
{
    public class QueryBaseControllers : System.Web.Mvc.Controller
    {
        protected QueryBaseDto GetRequestPage(QueryBaseDto qord)
        {
            if (Request.QueryString.AllKeys.Contains("next"))
            {
                qord.PageIndex = qord.PageIndex + 1;
            }
            else if (Request.QueryString.AllKeys.Contains("up"))
            {
                qord.PageIndex = qord.PageIndex - 1;
            }
            else if (Request.QueryString.AllKeys.Contains("first"))
            {
                qord.PageIndex = 1;
            }
            else if (Request.QueryString.AllKeys.Contains("last"))
            {
                qord.PageIndex = 999999999;
            }
            return qord;
        }
    }
}