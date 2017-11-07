using System;
using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace Ashx.ProductionProcessMgmt.Process
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
                        Insert(context);
                        break;
                    }
                default:
                    List(context);
                    break;
            }
        }

        void List(HttpContext context)
        {
            DbHelper db = new DbHelper();
            string strProductID = context.Request["product_id"];

            string sql = @"select cl.code_value as parts_no, cl.code1 - isnull(bc.comp_count,0) as qty
                                from hcode_line cl left join (select fbarcode, count(1) as comp_count
	                                from hbarcode bc
	                                where bc.productid = '{0}'
	                                group by fbarcode
	                                ) bc
                                on bc.fbarcode = code_value
	                                where cl.code_cate = 'barcode';";
            sql = string.Format(sql, strProductID);
            DataTable dt = db.ExecuteDataTable(sql);

            string strJson = JsonConvert.SerializeObject(dt);
            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        void Insert(HttpContext context)
        {
            Result rs = new Result();
            DbHelper db = new DbHelper();

            string strProductID = context.Request["product_id"];
            string strBarcode = context.Request["barcode"];

            string sql = @"insert into hbarcode 
                            (productid,fbarcode,fparts,sbarcode,sparts) values ('{0}','{1}','{2}','{3}','{4}');";
            sql = string.Format(sql, strProductID, strBarcode,"","","");

            if (db.ExecuteNonQuery(sql) != 1)
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