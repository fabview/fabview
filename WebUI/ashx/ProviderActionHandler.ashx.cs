using System;
using System.Web;
using System.Text;
using System.Data;
using System.Web.SessionState;
using FabViewModel;
using FabView.Utility;
using Newtonsoft.Json;
using System.Data.Common;

namespace Web.UI.FabView.Ashx.OrderManager
{
    /// <summary>
    /// ProviderActionHandler 的摘要说明
    /// </summary>
    public class ProviderActionHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];
            switch (action)
            {
                case "add":
                    {
                        Create(context);
                        break;
                    }
                case "edit":
                    {
                        Edit(context);
                        break;
                    }
                case "del":
                    {
                        Del(context);
                        break;
                    }
                case "split": //订单拆分 order split
                    {
                        Split(context);
                        break;
                    }
                case "list_sub_order": 
                    {
                        ListSubOrder(context);
                        break;
                    }
                case "del_sub_order":
                    {
                        DelSubOrder(context);
                        break;
                    }
                default:
                    break;
            }
        }

        private void Create(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var jsetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            Order order = (Order)JsonConvert.DeserializeObject<Order>(json, jsetting);//反序列化  

            String strUserName = context.Request["username"];
            string strSubOrder = order.ORDER_NUMBER + "-0001";

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetStoredProcCommond("spOrderOperate");
            db.AddInParameter(cmd, "@OPERATE_TYPE", DbType.String, "CREATE_ORDER");
            db.AddInParameter(cmd, "@ORDER_NUMBER", DbType.String, order.ORDER_NUMBER);
            db.AddInParameter(cmd, "@ORDER_TYPE", DbType.String, order.ORDER_TYPE);
            db.AddInParameter(cmd, "@PARTS_NO", DbType.String, order.PARTS_NO);
            db.AddInParameter(cmd, "@SUB_ORDER", DbType.String, strSubOrder);
            db.AddInParameter(cmd, "@SUB_ORDER_DESC", DbType.String, order.SUB_ORDER_DESC);
            db.AddInParameter(cmd, "@STATUS", DbType.String, "CREATED");
            db.AddInParameter(cmd, "@SALES_ORDER", DbType.String, order.SALES_ORDER);
            db.AddInParameter(cmd, "@CONTRACT", DbType.String, order.CONTRACT);
            db.AddInParameter(cmd, "@DISTRIBUTOR", DbType.String, order.DISTRIBUTOR);
            db.AddInParameter(cmd, "@DT_CONTACT", DbType.String, order.DT_CONTACT);
            db.AddInParameter(cmd, "@END_CUSTOMER", DbType.String, order.END_CUSTOMER);
            db.AddInParameter(cmd, "@END_CUST_SEX", DbType.String, order.END_CUST_SEX);
            db.AddInParameter(cmd, "@END_CUST_AGE", DbType.Int16, order.END_CUST_AGE);
            db.AddInParameter(cmd, "@SALES", DbType.String, order.SALES);
            db.AddInParameter(cmd, "@PROCESS_DAYS", DbType.Int16, order.PROCESS_DAYS);
            db.AddInParameter(cmd, "@ORDER_DATE", DbType.DateTime, DateTime.Now);
            db.AddInParameter(cmd, "@ESTIMATE_PACK_DATE", DbType.DateTime, DateTime.Now);
            db.AddInParameter(cmd, "@ACT_PACK_DATE", DbType.DateTime, DateTime.Now);
            db.AddInParameter(cmd, "@ACT_SHIP_DATE", DbType.DateTime, DateTime.Now);
            db.AddInParameter(cmd, "@NOTE", DbType.String, order.NOTE);
            db.AddInParameter(cmd, "@PICTURE", DbType.String, order.PICTURE);
            db.AddInParameter(cmd, "@PROD_CATE", DbType.String, order.PROD_CATE);
            db.AddInParameter(cmd, "@PROD_NAME", DbType.String, order.PROD_NAME);
            db.AddInParameter(cmd, "@PROCESS_TYPE", DbType.String, order.PROCESS_TYPE);
            db.AddInParameter(cmd, "@PROD_COLOR", DbType.String, order.PROD_COLOR);
            db.AddInParameter(cmd, "@PROD_POSITION", DbType.String, order.PROD_POSITION);
            db.AddInParameter(cmd, "@ATTACHMENT", DbType.String, order.ATTACHMENT);
            db.AddInParameter(cmd, "@USER", DbType.String, strUserName);

            Result rs = new Result();
            if (db.ExecuteNonQuery(cmd) == 1)
            {
                rs.status = 1;
                rs.msg = "添加成功！";
            }
            else
            {
                rs.status = 0;
                rs.msg = "服务器繁忙，请稍后再试！";
            }
            rs.item = "[]";
            string strJson = JsonConvert.SerializeObject(rs);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        private void Edit(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var jsetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            Order order = (Order)JsonConvert.DeserializeObject<Order>(json, jsetting);//反序列化  

            String strUserName = context.Request["username"];

            DbHelper db = new DbHelper();
            Result rs = new Result();
            DbCommand cmd = db.GetStoredProcCommond("spOrderOperate");
            db.AddInParameter(cmd, "@OPERATE_TYPE", DbType.String, "UPDATE_BY_ORDER");
            db.AddInParameter(cmd, "@ORDER_NUMBER", DbType.String, order.ORDER_NUMBER);
            db.AddInParameter(cmd, "@ORDER_TYPE", DbType.String, order.ORDER_TYPE);
            db.AddInParameter(cmd, "@PARTS_NO", DbType.String, order.PARTS_NO);
            db.AddInParameter(cmd, "@SUB_ORDER", DbType.String, order.SUB_ORDER);
            db.AddInParameter(cmd, "@SUB_ORDER_DESC", DbType.String, order.SUB_ORDER_DESC);
            db.AddInParameter(cmd, "@STATUS", DbType.String, "CREATED");
            db.AddInParameter(cmd, "@SALES_ORDER", DbType.String, order.SALES_ORDER);
            db.AddInParameter(cmd, "@CONTRACT", DbType.String, order.CONTRACT);
            db.AddInParameter(cmd, "@DISTRIBUTOR", DbType.String, order.DISTRIBUTOR);
            db.AddInParameter(cmd, "@DT_CONTACT", DbType.String, order.DT_CONTACT);
            db.AddInParameter(cmd, "@END_CUSTOMER", DbType.String, order.END_CUSTOMER);
            db.AddInParameter(cmd, "@END_CUST_SEX", DbType.String, order.END_CUST_SEX);
            db.AddInParameter(cmd, "@END_CUST_AGE", DbType.Int16, order.END_CUST_AGE);
            db.AddInParameter(cmd, "@SALES", DbType.String, order.SALES);
            db.AddInParameter(cmd, "@PROCESS_DAYS", DbType.Int16, order.PROCESS_DAYS);
            db.AddInParameter(cmd, "@ORDER_DATE", DbType.DateTime, DateTime.Now);
            db.AddInParameter(cmd, "@ESTIMATE_PACK_DATE", DbType.DateTime, DateTime.Now);
            db.AddInParameter(cmd, "@ACT_PACK_DATE", DbType.DateTime, DateTime.Now);
            db.AddInParameter(cmd, "@ACT_SHIP_DATE", DbType.DateTime, DateTime.Now);
            db.AddInParameter(cmd, "@NOTE", DbType.String, order.NOTE);
            db.AddInParameter(cmd, "@PICTURE", DbType.String, order.PICTURE);
            db.AddInParameter(cmd, "@PROD_CATE", DbType.String, order.PROD_CATE);
            db.AddInParameter(cmd, "@PROD_NAME", DbType.String, order.PROD_NAME);
            db.AddInParameter(cmd, "@PROCESS_TYPE", DbType.String, order.PROCESS_TYPE);
            db.AddInParameter(cmd, "@PROD_COLOR", DbType.String, order.PROD_COLOR);
            db.AddInParameter(cmd, "@PROD_POSITION", DbType.String, order.PROD_POSITION);
            db.AddInParameter(cmd, "@ATTACHMENT", DbType.String, order.ATTACHMENT);
            db.AddInParameter(cmd, "@USER", DbType.String, strUserName);

            if (db.ExecuteNonQuery(cmd) == 1)
            {
                rs.status = 1;
                rs.msg = "修改成功！";
            }
            else
            {
                rs.status = 0;
                rs.msg = "服务器繁忙，请稍后再试！";
            }
            rs.item = "[]";
            string strJson = JsonConvert.SerializeObject(rs);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        private void Split(HttpContext context)
        {
            DbHelper db = new DbHelper();
            Result rs = new Result();

            string sql = "select count(1) from horder where sub_order = '{0}';";
            sql = string.Format(sql, context.Request["sub_order"]);
            int count = db.ExecuteScalar(sql);
            if (count > 0)
            {
                sql = @"update horder 
                            set qty={0},sub_order_desc='{1}',last_updated_date=getdate(),last_updated_by='{2}' 
                                where sub_order = '{3}';";
                sql = string.Format(sql, context.Request["qty"], context.Request["sub_order_desc"], context.Request["username"],
                    context.Request["sub_order"]);

                if (db.ExecuteNonQuery(sql) == 1)
                {
                    rs.status = 1;
                    rs.msg = "子订单修改成功！";
                }
                else
                {
                    rs.status = 0;
                    rs.msg = "服务器繁忙，请稍后再试！";
                }
            }
            else
            {
                DbCommand cmd = db.GetStoredProcCommond("spOrderOperate");
                db.AddInParameter(cmd, "@OPERATE_TYPE", DbType.String, "CREATE_SUB_ORDER");
                db.AddInParameter(cmd, "@ORDER_NUMBER", DbType.String, context.Request["order_number"]);
                db.AddInParameter(cmd, "@SUB_ORDER", DbType.String, context.Request["sub_order"]);
                db.AddInParameter(cmd, "@SUB_ORDER_DESC", DbType.String, context.Request["sub_order_desc"]);
                db.AddInParameter(cmd, "@QTY", DbType.Int16, context.Request["qty"]);
                db.AddInParameter(cmd, "@USER", DbType.String, context.Request["username"]);

                db.AddInParameter(cmd, "@PROD_CATE", DbType.String, context.Request["prod_cate"]);
                db.AddInParameter(cmd, "@PROD_NAME", DbType.String, context.Request["prod_name"]);
                db.AddInParameter(cmd, "@PROCESS_TYPE", DbType.String, context.Request["process_type"]);
                db.AddInParameter(cmd, "@PROD_COLOR", DbType.String, context.Request["product_color"]);
                db.AddInParameter(cmd, "@PROD_POSITION", DbType.String, context.Request["product_position"]);

                if (db.ExecuteNonQuery(cmd) >= 1)
                {
                    rs.status = 1;
                    rs.msg = "订单拆分成功！";
                }
                else
                {
                    rs.status = 0;
                    rs.msg = "服务器繁忙，请稍后再试！";
                }
            }

            rs.item = "[]";
            string strJson = JsonConvert.SerializeObject(rs);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        private void Del(HttpContext context)
        {
            var orderid = HttpContext.Current.Request["id"];

            string sql = "delete from horder where orderid in ({0})";
            sql = string.Format(sql, orderid);
            Result rs = new Result();
            DbHelper db = new DbHelper();
            if (db.ExecuteNonQuery(sql) >= 0)
            {
                rs.status = 1;
                rs.msg = "删除成功！";
            }
            else
            {
                rs.status = 0;
                rs.msg = "服务器繁忙，请稍后再试！";
            }
            rs.item = "[]";
            string strJson = JsonConvert.SerializeObject(rs);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        void ListSubOrder(HttpContext context)
        {
            DbHelper db = new DbHelper();
            string sql = @"select sub_order,
                                   sub_order_desc,
                                   qty,
                                   orderid,
                                   prod_cate,
                                   prod_name,
                                   process_type,
                                   prod_color,
                                   prod_position
                              from horder
                             where order_number = '{0}'
                             order by sub_order";
            sql = string.Format(sql, context.Request["order_number"]);
            DataTable dt = db.ExecuteDataTable(sql);
            string strJson = JsonConvert.SerializeObject(dt);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        void DelSubOrder(HttpContext context)
        {
            string sql = "delete from horder where sub_order in ('{0}');";
            sql = string.Format(sql, context.Request["sub_order"]);
            Result rs = new Result();
            DbHelper db = new DbHelper();
            if (db.ExecuteNonQuery(sql) == 1)
            {
                rs.status = 1;
                rs.msg = "保存成功！";
            }
            else
            {
                rs.status = 0;
                rs.msg = "服务器繁忙，请稍后再试！";
            }
            rs.item = "[]";
            string strJson = JsonConvert.SerializeObject(rs);

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
