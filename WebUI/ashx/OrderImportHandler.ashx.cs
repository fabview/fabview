using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System;
using System.Data.Common;

namespace ZGZY.WebUI.ashx {
    /// <summary>
    /// OrderImportHandler 的摘要说明
    /// </summary>
    public class OrderImportHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";

            var action = context.Request["action"];
            switch (action) {
                case "import": {
                        OrderImport(context);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 导入订单（通过xml文件）
        /// </summary>
        /// <param name="context"></param>
        void OrderImport(HttpContext context) {
            Result rs = new Result();
            DbHelper db = new DbHelper();

            //Stream s = context.Request.InputStream; 
            //byte[] b = new byte[s.Length];
            //s.Read(b, 0, (int)s.Length);
            //string str =  Encoding.UTF8.GetString(b);

            string sql = string.Empty;
            string strUserName = context.Request["username"];
            string Path = context.Request["path"].TrimEnd('\\');

            DirectoryInfo TheFolder = new DirectoryInfo(Path);
            foreach (FileInfo NextFile in TheFolder.GetFiles()) {
                if ((NextFile.Extension).ToUpper() != ".XML") {
                    continue;
                }

                DataSet ds = new DataSet();
                ds.ReadXml(Path + "\\" + NextFile.Name);
                DataTable dtMainOrderInfo = ds.Tables["basicInfo"];
                DataTable dtSubOrderInfo = ds.Tables["task"];

                sql = @"delete from hPrdRoute where wo_number in 
                            (select sub_order from horder where order_number='"+ dtMainOrderInfo.Rows[0]["orderNo"] + "')";
                sql = @"delete from horder where order_number='" + dtMainOrderInfo.Rows[0]["orderNo"] + "';";
                db.ExecuteNonQuery(sql);

                foreach (DataRow dr in dtSubOrderInfo.Rows) {
                    DbCommand cmd = db.GetStoredProcCommond("spOrderOperate");
                    db.AddInParameter(cmd, "@OPERATE_TYPE", DbType.String, "CREATE_ORDER");
                    db.AddInParameter(cmd, "@ORDER_NUMBER", DbType.String, dtMainOrderInfo.Rows[0]["orderNo"]);
                    db.AddInParameter(cmd, "@ORDER_TYPE", DbType.String, dtMainOrderInfo.Rows[0]["orderName"]);
                    //db.AddInParameter(cmd, "@PARTS_NO", DbType.String, order.PARTS_NO);
                    db.AddInParameter(cmd, "@SUB_ORDER", DbType.String, dtMainOrderInfo.Rows[0]["orderNo"] + "_" + dr["teethPosition"].ToString().Trim());
                    //db.AddInParameter(cmd, "@SUB_ORDER_DESC", DbType.String, order.SUB_ORDER_DESC);
                    db.AddInParameter(cmd, "@STATUS", DbType.String, "CREATED");
                    //db.AddInParameter(cmd, "@SALES_ORDER", DbType.String, order.SALES_ORDER);
                    //db.AddInParameter(cmd, "@CONTRACT", DbType.String, order.CONTRACT);
                    db.AddInParameter(cmd, "@DISTRIBUTOR", DbType.String, dtMainOrderInfo.Rows[0]["company"]);
                    db.AddInParameter(cmd, "@DT_CONTACT", DbType.String, dtMainOrderInfo.Rows[0]["truename"]);
                    db.AddInParameter(cmd, "@END_CUSTOMER", DbType.String, dtMainOrderInfo.Rows[0]["patientName"]);
                    db.AddInParameter(cmd, "@END_CUST_SEX", DbType.String, dtMainOrderInfo.Rows[0]["patientSex"]);
                    db.AddInParameter(cmd, "@END_CUST_AGE", DbType.Int16, dtMainOrderInfo.Rows[0]["patientAge"]);
                    //db.AddInParameter(cmd, "@SALES", DbType.String, order.SALES);
                    //db.AddInParameter(cmd, "@PROCESS_DAYS", DbType.Int16, order.PROCESS_DAYS);
                    db.AddInParameter(cmd, "@ORDER_DATE", DbType.DateTime, DateTime.Parse(dtMainOrderInfo.Rows[0]["orderCreateTime"].ToString()));
                    db.AddInParameter(cmd, "@ESTIMATE_PACK_DATE", DbType.DateTime, DateTime.Parse(dtMainOrderInfo.Rows[0]["receiveTime"].ToString()));
                    //db.AddInParameter(cmd, "@ACT_PACK_DATE", DbType.DateTime, DateTime.Now);
                    //db.AddInParameter(cmd, "@ACT_SHIP_DATE", DbType.DateTime, DateTime.Now);
                    //db.AddInParameter(cmd, "@NOTE", DbType.String, order.NOTE);
                    //db.AddInParameter(cmd, "@PICTURE", DbType.String, order.PICTURE);
                    //db.AddInParameter(cmd, "@PROD_CATE", DbType.String, order.PROD_CATE);
                    //db.AddInParameter(cmd, "@PROD_NAME", DbType.String, order.PROD_NAME);
                    //db.AddInParameter(cmd, "@PROCESS_TYPE", DbType.String, order.PROCESS_TYPE);
                    db.AddInParameter(cmd, "@PROD_COLOR", DbType.String, dr["teethColor"]);
                    db.AddInParameter(cmd, "@PROD_POSITION", DbType.String, dr["teethPosition"]);
                    //db.AddInParameter(cmd, "@ATTACHMENT", DbType.String, order.ATTACHMENT);
                    db.AddInParameter(cmd, "@USER", DbType.String, strUserName);

                    db.ExecuteNonQuery(cmd);
                }

                rs.status = 1;
                rs.msg += NextFile.Name + ": " + "添加成功！\r\n";

            }

            string strJson = JsonConvert.SerializeObject(rs);
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