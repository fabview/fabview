using System;
using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace Web.UI.FabView.Ashx.ProductionProcessMgmt.Process
{
    /// <summary>
    /// ListPartsNo 的摘要说明
    /// </summary>
    public class ListPartsNo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action)
            {
                case "list":
                    {
                        List(context);
                        break;
                    }
                case "insert":
                    {
                        BarcodeScan(context);
                        break;
                    }
                default:
                    List(context);
                    break;
            }
        }

        /// <summary>
        /// 查询待采集的物料清单
        /// </summary>
        /// <param name="context"></param>
        void List(HttpContext context)
        {
            DbHelper db = new DbHelper();
            string strWoNumber = context.Request["wo_number"];
            string strProcess = context.Request["process_name"];

            /*string sql = @"select cl.code_value as parts_no, cl.code1 - bc.comp_count as qty
                                from hcode_line cl, (select fbarcode, count(1) as comp_count
	                                from hbarcode bc
	                                where bc.productid = '{0}'
	                                group by fbarcode
	                                ) bc
                                where cl.code_cate = 'barcode'
	                                and bc.fbarcode = code_value;";*/

            //先注释这段逻辑，临时使用按需采集条码的逻辑
            //string sql = @"SELECT t.parts_no, 
            //                       ( (SELECT TOP 1 code1 
            //                          FROM   hcode_line 
            //                          WHERE  code_cate = 'barcode'/* and code2=t.PROCESS_NAME*/) - 
            //                         Isnull(bc.comp_count, 0) ) AS qty 
            //                FROM   htasks t 
            //                       LEFT JOIN (SELECT fbarcode, 
            //                                         Count(1) AS comp_count 
            //                                  FROM   hbarcode bc 
            //                                  WHERE  bc.wo_number = '{0}' 
            //                                  GROUP  BY fbarcode) bc 
            //                              ON bc.fbarcode = t.parts_no 
            //                WHERE  t.process_name = '{1}' 
            //                       AND t.task_type = 'BARCODE'
            //                       AND t.wo_number='{0}'";

            //string sql = @"select parts_no, null as qty from htasks 
            //            where wo_number='{0}' and process_name = '{1}' and task_type = 'BARCODE'";ing 
            string sql = @"select fbarcode as parts_no,1 as qty,fbarcode as barcode from hbarcode where wo_number='{0}' and process_name = '{1}'";

            sql = string.Format(sql, strWoNumber, strProcess);
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
        /// 条码采集
        /// </summary>
        /// <param name="context"></param>
        void BarcodeScan(HttpContext context)
        {
            Result rs = new Result();
            DbHelper db = new DbHelper();

            string strWoNumber = context.Request["product_id"];
            string strBarcode = context.Request["barcode"];
            string strProcess_name = context.Request["process_name"];
            string strUserName = context.Request["user_name"];

            string sql = @"insert into hbarcode 
                            (wo_number,process_name,fbarcode,fparts,sbarcode,sparts,creation_date,created_by) 
                            values ('{0}','{1}','{2}','{3}','{4}','{5}',getdate(),'{6}');";
            sql = string.Format(sql, strWoNumber, strProcess_name, strBarcode,"","","", strUserName);

            string sql_update_tasks = @"update htasks set performed_by='{0}',performed_date=getdate() 
                        where wo_number='{1}' and process_name='{2}' and task = '{3}'";
            sql_update_tasks = string.Format(sql_update_tasks, strUserName, strWoNumber, strProcess_name, strBarcode);
            db.ExecuteNonQuery(sql_update_tasks);

            if (db.ExecuteNonQuery(sql) < 1)
            {
                rs.status = 0;
                rs.msg = "服务器繁忙，请稍后再试！";
            } else {
                rs.status = 1;
                rs.msg = "";
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