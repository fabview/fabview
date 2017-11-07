using System;
using System.Text;
using System.Web;
using System.Data;
using FabView.Utility;
using System.Data.Common;


namespace ZGZY.WebUI.ashx {
    /// <summary>
    /// OrderHandler1 的摘要说明
    /// </summary>
    public class OrderHandler1 : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "query": {
                        Query(context);
                        break;
                    }
                case "query_order_production_info": {
                        QueryOrderPrdInfo(context);
                        break;
                    }
                default:
                    break;
            }
        }

        void Query(HttpContext context) {
            int pageSize = Convert.ToInt32(context.Request["rows"]);//通过这个获取得到pageSize  
            int pageNum = Convert.ToInt32(context.Request["page"]);//通过这个获取得到pageNum（当前页码） 
            string order_number = context.Request["order_number"];
            string sub_order = context.Request["sub_order"];
            string cur_process = context.Request["cur_process"];
            string next_process = context.Request["next_process"];
            string status = context.Request["status"];
            string order_date_begin = context.Request["order_date_begin"];
            string order_date_end = context.Request["order_date_end"];
            string distributor = context.Request["distributor"];
            string dt_contact = context.Request["dt_contact"];
            string end_customer = context.Request["end_customer"];

            var sort = context.Request["sort"];//排序字段  
            var order = context.Request["order"];//升序降序  

            string condition = " where 1=1";//查询条件  
            if (!string.IsNullOrEmpty(order_number)) {
                condition += " and order_number like '" + order_number.Replace('*', '%') + "'";
            }
            if (!string.IsNullOrEmpty(sub_order)) {
                condition += " and sub_order like '" + sub_order.Replace('*', '%') + "'";
            }
            if (!string.IsNullOrEmpty(cur_process)) {
                condition += " and cur_process like '" + cur_process.Replace('*', '%') + "'";
            }
            if (!string.IsNullOrEmpty(next_process)) {
                condition += " and next_process like '" + next_process.Replace('*', '%') + "'";
            }
            if (!string.IsNullOrEmpty(status)) {
                condition += " and status like '" + status.Replace('*', '%') + "'";
            }
            if (!string.IsNullOrEmpty(order_date_begin)) {
                condition += " and order_date >= '" + order_date_begin + "'";
            }
            if (!string.IsNullOrEmpty(order_date_end)) {
                condition += " and order_date <= '" + order_date_end + "'";
            }
            if (!string.IsNullOrEmpty(distributor)) {
                condition += " and distributor like '" + distributor.Replace('*', '%') + "'";
            }
            if (!string.IsNullOrEmpty(dt_contact)) {
                condition += " and dt_contact like '" + dt_contact.Replace('*', '%') + "'";
            }
            if (!string.IsNullOrEmpty(end_customer)) {
                condition += " and end_customer like '" + end_customer.Replace('*', '%') + "'";
            }

            string sqlcount = "select count(1) from horder" + condition;
            string sql = "select * from horder" + condition;

            string mysort = "sub_order";//排序情况  
            if (!string.IsNullOrEmpty(sort)) {
                mysort = sort + " " + order;
            }

            DbHelper db = new DbHelper();
            int count = db.ExecuteScalar(sqlcount); //总行数，用于分页

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

        void QueryOrderPrdInfo(HttpContext context) {
            int pageSize = Convert.ToInt32(context.Request["rows"]);//通过这个获取得到pageSize  
            int pageNum = Convert.ToInt32(context.Request["page"]);//通过这个获取得到pageNum（当前页码） 
            //string order_number = context.Request["order_number"];
            string sub_order = context.Request["sub_order"];

            var sort = context.Request["sort"];//排序字段  
            var order = context.Request["order"];//升序降序  

            string condition = " where 1=1";//查询条件  
            if (!string.IsNullOrEmpty(sub_order)) {
                condition += " and wo_number like '" + sub_order.Replace('*', '%') + "'";
            }
            

            string sqlcount = "select count(1) from hprdroute" + condition;
            string sql = "select * from hprdroute" + condition;

            string mysort = "wo_number,sequence";//排序情况  
            if (!string.IsNullOrEmpty(sort)) {
                mysort = sort + " " + order;
            }

            DbHelper db = new DbHelper();
            int count = db.ExecuteScalar(sqlcount); //总行数，用于分页

            DbCommand cmd = db.GetStoredProcCommond("spPagingQuery");
            db.AddInParameter(cmd, "@SQL", DbType.String, sql);
            db.AddInParameter(cmd, "@Page", DbType.Int16, pageNum);
            db.AddInParameter(cmd, "@RecordsPerPage", DbType.Int16, pageSize);
            db.AddInParameter(cmd, "@ID", DbType.String, "ID");
            db.AddInParameter(cmd, "@Sort", DbType.String, mysort);
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

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}