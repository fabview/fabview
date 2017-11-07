using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace ZGZY.WebUI.ashx {
    /// <summary>
    /// TestHandler 的摘要说明
    /// </summary>
    public class TestHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "select": {
                        Select(context);
                        break;
                    }
                case "create": {
                        Create(context);
                        break;
                    }
                default:
                    break;
            }
        }

        void Select(HttpContext context) {
            DbHelper db = new DbHelper();
            //xujiyuan            string sql = @"SELECT CL.ID, CL.CODE_VALUE, CL.CODE_DESC, CL.CODE1,CREATION_DATE, CASE WHEN PROCESS_NAME IS NULL THEN 0 ELSE 1 END  AS IS_ASSIGN
            //                            FROM (SELECT ID, CODE_VALUE, CODE_DESC, CODE1, CREATION_DATE
            //                                FROM HCODE_LINE
            //                                WHERE CODE_CATE = 'PROCESS_STEP'
            //                                ) CL
            //                                LEFT JOIN (SELECT PROCESS_NAME
            //                                    FROM HROUTE
            //                                    WHERE WO_NUMBER = '{0}'
            //                                    ) R ON R.PROCESS_NAME = CL.CODE_VALUE ORDER BY CODE1 ASC;";

            //xujiyuan            sql = string.Format(sql, "123"/*context.Request["order_number"]*/);

            string sql = "SELECT ID, CODE_VALUE, CODE_DESC, CODE1, CREATION_DATE from HCODE_LINE WHERE CODE_CATE = 'PROCESS_STEP'";
            DataTable dt = db.ExecuteDataTable(sql);
            string strJson = JsonConvert.SerializeObject(dt);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        void Create(HttpContext context) {
            string strJson;
            Result rs = new Result();
            DbHelper db = new DbHelper();
            var id = HttpContext.Current.Request["id"];
            var sub_number = HttpContext.Current.Request["wo_number"];

            string sql = "delete from HTestRoute where wo_number in ('{0}');";
            sql = string.Format(sql, sub_number);
            if (db.ExecuteNonQuery(sql) == -1) {
                rs.status = 0;
                rs.msg = "服务器繁忙，请稍后再试！";

                //将输出传递到前台
                strJson = JsonConvert.SerializeObject(rs);
                context.Response.Clear();
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentType = "application/json";
                context.Response.Write(strJson);
                context.Response.Flush();
                context.Response.End();
            }

            sql = @"INSERT INTO HTestRoute 
                        SELECT '{0}', CODE1, CODE_VALUE ,row_number() over(order by code_cate), GETDATE(), 'admin' ,NULL,NULL
                            FROM HCODE_LINE WHERE CODE_CATE='PROCESS_STEP' AND ID IN ({1})";

            sql = string.Format(sql, sub_number, id);


            if (db.ExecuteNonQuery(sql) >= 1) {
                rs.status = 1;
                rs.msg = "添加成功！";
            } else {
                rs.status = 0;
                rs.msg = "服务器繁忙，请稍后再试！";
            }
            rs.item = "[]";


  
                strJson = JsonConvert.SerializeObject(rs);
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