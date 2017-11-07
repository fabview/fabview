using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace ZGZY.WebUI.ashx {
    /// <summary>
    /// ListProcess 的摘要说明
    /// </summary>
    public class ListProcess : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "list": {
                        List(context);
                        break;
                    }
                default:
                    break;
            }
        }

        void List(HttpContext context) {
            DbHelper db = new DbHelper();
            string sql = @"SELECT * FROM HPROCESS WHERE ISNULL(ENABLED, 'Y') <> 'N' ORDER BY PROCESS_ID";

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