using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using System.Data.Common;

namespace ZGZY.WebUI.ashx {
    /// <summary>
    /// WorkingInstructionHandler 的摘要说明
    /// </summary>
    public class WorkingInstructionHandler : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action) {
                case "list": {
                        List(context);
                        break;
                    }
                case "save": {
                        Save(context);
                        break;
                    }
            }
        }

        void List(HttpContext context) {
            DbHelper db = new DbHelper();
            string sProcessCode = context.Request["process_code"];
            string sPartsNo = context.Request["parts_no"];
            string sUser = context.Request["user_name"];

            string sql = "select * from hwi where 1=1";  
            if (!string.IsNullOrEmpty(sProcessCode)) {
                sql += " and process_code like '" + sProcessCode.Replace('*', '%') + "'";
            }
            if (!string.IsNullOrEmpty(sPartsNo)) {
                sql += " and parts_no like '" + sPartsNo.Replace('*', '%') + "'";
            }

            DataTable dt = db.ExecuteDataTable(sql);

            //var sort = context.Request["sort"];//排序字段  
            //var order = context.Request["order"];//升序降序  

            //string condition = " where 1=1";//查询条件  
            //if (!string.IsNullOrEmpty(sProcessCode)) {
            //    condition += " and process_code like '" + sProcessCode.Replace('*', '%') + "'";
            //}
            //if (!string.IsNullOrEmpty(sPartsNo)) {
            //    condition += " and parts_no like '" + sPartsNo.Replace('*', '%') + "'";
            //}         

            //string sqlcount = "select count(1) from horder" + condition;
            //string sql = "select * from horder" + condition;

            //string mysort = "sub_order";//排序情况  
            //if (!string.IsNullOrEmpty(sort)) {
            //    mysort = sort + " " + order;
            //}

            //DbHelper db = new DbHelper();
            //int count = db.ExecuteScalar(sqlcount); //总行数，用于分页

            //DbCommand cmd = db.GetStoredProcCommond("spPagingQuery");
            //db.AddInParameter(cmd, "@SQL", DbType.String, sql);
            //db.AddInParameter(cmd, "@Page", DbType.Int16, pageNum);
            //db.AddInParameter(cmd, "@RecordsPerPage", DbType.Int16, pageSize);
            //db.AddInParameter(cmd, "@ID", DbType.String, "ORDERID");
            //db.AddInParameter(cmd, "@Sort", DbType.String, "SUB_ORDER DESC");
            //DataTable dt = db.ExecuteDataTable(cmd);

            //System.Collections.Hashtable ht = new System.Collections.Hashtable();
            //ht.Add("total", count);
            //ht.Add("rows", dt);
            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(dt);//序列化datatable  

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        void Save(HttpContext context) {
            string sProcessID = context.Request["process_id"];
            string sPartsNo = context.Request["parts_no"];
            string sWI = context.Request["wi"];
            string sUser = context.Request["user_name"];

            DbHelper db = new DbHelper();
            DbCommand cmd = db.GetStoredProcCommond("spWIInsert");
            db.AddInParameter(cmd, "@process_id", DbType.String, sProcessID);
            db.AddInParameter(cmd, "@parts_no", DbType.String, sPartsNo);
            db.AddInParameter(cmd, "@wi", DbType.String, sWI);
            db.AddInParameter(cmd, "@user", DbType.String, sUser);

            Result rs = new Result();
            if (db.ExecuteNonQuery(cmd) == 1) {
                rs.status = 1;
                rs.msg = "添加成功！";
            } else {
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

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}