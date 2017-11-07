using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace Web.UI.FabView.Ashx.ProductionPlanMgmt.Route {
    /// <summary>
    /// RouteHandler 的摘要说明
    /// </summary>
    public class RouteHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "select": {
                        Select(context);
                        break;
                    }
                case "select_all": {
                        SelectAll(context);
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

        /// <summary>
        /// 查询全部工序
        /// 对订单已经选择的工序，反填到DataGrid
        /// </summary>
        /// <param name="context"></param>
        void Select(HttpContext context) {
            DbHelper db = new DbHelper();
            string strSubOrder = context.Request["sub_order"];
            string sql = @"SELECT p.ID, p.PROCESS_NAME, p.PROCESS_ID, p.PROCESS_DESC, p.TOOL_NAME, p.TOOL_DESC, p.TOOL_GROUP,
                                  case when r.process_name is null then 0 else 1 end as ASSIGNED
                            FROM   hprocess p 
                                    LEFT JOIN (SELECT process_id, process_name 
                                                FROM   hprdroute 
                                                WHERE  wo_number = '{0}') r 
                                            ON p.process_id = r.process_id 
                            ORDER  BY p.process_id ";

            sql = string.Format(sql, strSubOrder);
            DataTable dt = db.ExecuteDataTable(sql);
            string strJson = JsonConvert.SerializeObject(dt);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        void SelectAll(HttpContext context) {
            DbHelper db = new DbHelper();
            string sql = @"select p.ID, p.PROCESS_NAME, p.PROCESS_ID, p.PROCESS_DESC, p.TOOL_NAME, p.TOOL_DESC, p.TOOL_GROUP
                            from hprocess p order by p.process_id ";

            DataTable dt = db.ExecuteDataTable(sql);
            string strJson = JsonConvert.SerializeObject(dt);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        /// <summary>
        /// 创建订单工艺路线
        /// </summary>
        /// <param name="context"></param>
        void Create(HttpContext context) {
            string _service = "RouteHandler/Create \r\n \r\n";
            string strJson;
            Result rs = new Result();
            DbHelper db = new DbHelper();
            var id = HttpContext.Current.Request["id"];
            var sub_orders = HttpContext.Current.Request["sub_order"];

            string sql = "delete from hprdroute where wo_number in ({0});";
            sql = string.Format(sql, sub_orders);
            if (db.ExecuteNonQuery(sql) == -1) {
                rs.status = 0;
                rs.msg = "工序添加失败！</br>" + "程序名：RouteHandler/Create";
                rs.item = "[]";
                strJson = JsonConvert.SerializeObject(rs);

                context.Response.Clear();
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentType = "application/json";
                context.Response.Write(strJson);
                context.Response.Flush();
                context.Response.End();
                return;
            }

            //sql = @"insert into hprdroute 
            //            select '{0}', CODE1, CODE_VALUE ,row_number() over(order by code_cate), GETDATE(), 'admin' ,NULL,NULL
            //                FROM HCODE_LINE WHERE CODE_CATE='PROCESS_STEP' ID IN ({1})";
            sql = @"insert into hprdroute 
                                (wo_number, 
                                 process_id, 
                                 process_name, 
                                 sequence, 
                                 creation_date, 
                                 created_by) 
                    select {0}, 
                            process_id, 
                            process_name, 
                            Row_number() OVER(ORDER BY process_id), 
                            Getdate(), 
                            'RouteHandler/Create' 
                    from   hprocess
                    where  id in ( {1} ); ";

            string sql_update_order = @"update horder 
                                            set next_process=(select process_name from hprdroute where sequence=1 and wo_number={0})
                                        where sub_order={0};";

            string _sql = string.Empty;
            string sql_update_order_t = string.Empty;

            foreach (string sub_order in sub_orders.Split(',')) {
                _sql += string.Format(sql, sub_order, id) + "\r\n";
                sql_update_order_t += string.Format(sql_update_order, sub_order) + "\r\n";
            }

            if (db.ExecuteNonQuery(_sql, rs) > 1) {
                rs.status = 1;
                rs.msg = "添加成功！";
                rs.item = "[]";
            } else if (db.ExecuteNonQuery(sql_update_order_t, rs) > 1) {

            } else {
                rs.status = 0;
                rs.msg = _service + rs.msg + _sql;
                rs.item = "{\"wo_number\":[{\"wo_number\":\"" + sub_orders + "\"}]}";
            }

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