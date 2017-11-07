using System;
using System.Text;
using System.Web;
using System.Data;
using FabView.Utility;
using System.Data.Common;

namespace Web.UI.FabView.Ashx.OrderManager
{
    /// <summary>
    /// ProviderHandler 的摘要说明
    /// </summary>
    public class ProviderHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int pageSize = Convert.ToInt32(context.Request["rows"]);//通过这个获取得到pageSize  
            int pageNum = Convert.ToInt32(context.Request["page"]);//通过这个获取得到pageNum  
            string keyname = context.Request["searchKey"];//查询的关键字  
            string keyvalue = context.Request["searchValue"];//查询的字段  

            var sort = context.Request["sort"];//排序字段  
            var order = context.Request["order"];//升序降序  

            string condition = " where 1=1";//查询条件  
            if (keyname != null)
            {
                condition += " and " + keyname + " like '%" + keyvalue + "%'";
            }

            string sqlcount = "select count(1) from horder" + condition;
            string sql = "select * from horder" + condition;

            string mysort = "sub_order";//排序情况  
            if (sort != null)
            {
                mysort = sort + " " + order;
            }

            DbHelper db = new DbHelper();
            int count = db.ExecuteScalar(sqlcount);

            DbCommand cmd = db.GetStoredProcCommond("spPagingQuery");
            db.AddInParameter(cmd, "@SQL", DbType.String, sql);
            db.AddInParameter(cmd, "@Page", DbType.Int16, pageNum);
            db.AddInParameter(cmd, "@RecordsPerPage", DbType.Int16, pageSize);
            db.AddInParameter(cmd, "@ID", DbType.String, "ORDERID");
            db.AddInParameter(cmd, "@Sort", DbType.String, "SUB_ORDER DESC");
            DataTable dt = db.ExecuteDataTable(cmd);

            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            ht.Add("total", count);
            ht.Add("rows", dt);
            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ht);//序列化datatable  


            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}