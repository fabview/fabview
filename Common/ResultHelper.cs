using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using FabView.Utility;

namespace ZGZY.Common {
    public class ResultHelper {
        public string GenMsg(string service, string msg, string sql) {
            service += "\r\n \r\n";
            msg += "\r\n \r\n";

            return service + msg + sql;
        }

        public string GenItem(string subOrder) {
            return "{\"wo_number\":[{\"wo_number\":\"" + subOrder + "\"}]}"; ;
        }

        public void Response(HttpContext context,Result rs) {
            string strJson = JsonConvert.SerializeObject(rs);
            strJson = JsonConvert.SerializeObject(rs);
            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }
    }
}
