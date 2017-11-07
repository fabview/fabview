using System;
using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace ZGZY.WebUI.fabview.ashx.zzCommon {
    /// <summary>
    /// ListCode 的摘要说明
    /// </summary>
    public class ListCode : IHttpHandler {

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
            string codeCate = context.Request["code_cate"].ToUpper();
            string sql = @"SELECT * FROM HCODE_LINE WHERE CODE_CATE='{0}' ORDER BY CREATION_DATE DESC";
            sql = string.Format(sql, codeCate);

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