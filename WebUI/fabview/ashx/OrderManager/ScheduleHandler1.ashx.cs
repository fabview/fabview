using System;
using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;


namespace Ashx.OrderManager {
    /// <summary>
    /// ProviderHandler 的摘要说明
    /// </summary>
    public class ScheduleHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "list": {
                        ListOrder(context);
                        break;
                    }
                default:
                    break;
            }
        }

        void ListOrder(HttpContext context) {
            var orderNumber = context.Request["order_number"];
            var subOrder = context.Request["sub_order"];
            var EPDBegin = context.Request["ESTIMATE_PACK_DATE_BEGIN"];
            var EPDEnd = context.Request["ESTIMATE_PACK_DATE_END"];
            string sql = @"select * from horder where 1=1 ";

            if (!string.IsNullOrEmpty(orderNumber)) {
                sql += "and order_number like '%" + orderNumber + "%'";
            }
            if (!string.IsNullOrEmpty(EPDBegin)) {
                sql += "and ESTIMATE_PACK_DATE>='" + EPDBegin + "'";
            }
            if (!string.IsNullOrEmpty(EPDEnd)) {
                sql += "and ESTIMATE_PACK_DATE<='" + EPDEnd + "'";
            }
            if (!string.IsNullOrEmpty(subOrder)) {
                sql += "and order_number='" + subOrder + "'";
            }

            DbHelper db = new DbHelper();
            DataTable dt = db.ExecuteDataTable(sql);
            string strJson = JsonConvert.SerializeObject(dt);

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