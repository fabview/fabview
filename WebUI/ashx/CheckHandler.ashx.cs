using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace ZGZY.WebUI.ashx {
    /// <summary>
    /// CheckHandler 的摘要说明
    /// </summary>
    public class CheckHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "check_sub_order": {
                        CheckSubOrder(context);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 检查子订单是否存在
        /// </summary>
        /// <param name="context"></param>
        void CheckSubOrder(HttpContext context) {
            Result rs = new Result();
            DbHelper db = new DbHelper();
            string subOrder = context.Request["sub_order"].ToUpper();
            string sql = @"SELECT * FROM HORDER WHERE SUB_ORDER='{0}'";
            sql = string.Format(sql, subOrder);

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