using System;
using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace Web.UI.FabView.Ashx.BasicData
{
    /// <summary>
    /// CodeList 的摘要说明
    /// </summary>
    public class CodeList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var action = context.Request["action"];

            switch (action)
            {
                case "select":
                    {
                        Select(context);
                        break;
                    }
                case "select_lines":
                    {
                        SelectLines(context);
                        break;
                    }
                case "insert":
                    {
                        Insert(context);
                        break;
                    }
                case "delete_code":
                    {
                        DeleteCode(context);
                        break;
                    }
                case "insert_lines":
                    {
                        InsertLines(context);
                        break;
                    }
                case "delete_lines":
                    {
                        DeleteLines(context);
                        break;
                    }
                default:
                    break;
            }
        }

        void Select(HttpContext context)
        {
            DbHelper db = new DbHelper();
            string sql = @"SELECT * FROM HCODE ORDER BY CREATION_DATE DESC";
            DataTable dt = db.ExecuteDataTable(sql);
            string strJson = JsonConvert.SerializeObject(dt);

            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.Write(strJson);
            context.Response.Flush();
            context.Response.End();
        }

        void SelectLines(HttpContext context)
        {
            DbHelper db = new DbHelper();
            string sql = @"SELECT * FROM HCODE_LINE WHERE 1 = 1 ";
            string codeCate = context.Request["code_cate"];
            if (!string.IsNullOrEmpty(codeCate))
                sql += "and code_cate='" + codeCate + "'";


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
            string sql = "select count(1) from hcode where code_cate = '{0}';";
            sql = string.Format(sql, context.Request["code_cate"]);

            string strCodeCate = context.Request["code_cate"];
            string strDesc = context.Request["desc"];
            string strIsSystem = context.Request["is_system"];
            string strUser = context.Request["created_by"];

            int count = db.ExecuteScalar(sql);
            if (count > 0)
            {
                sql = @"update hcode 
                            set is_system='{0}',[desc]='{1}',last_updated_date=getdate(),last_updated_by='{2}' 
                                where code_cate = '{3}'";
                sql = string.Format(sql, strIsSystem, strDesc, strUser, strCodeCate);
            }
            else
            {
                sql = "insert into hcode (code_cate,is_system,[desc],creation_date,created_by) values ('{0}','{1}','{2}','{3}','{4}');";
                sql = string.Format(sql, strCodeCate, strIsSystem, strDesc, DateTime.Now, strUser);
            }
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

        void InsertLines(HttpContext context)
        {
            Result rs = new Result();
            DbHelper db = new DbHelper();

            string strID = context.Request["ID"];
            string strCodeCate = context.Request["code_cate"];
            string strCodeValue = context.Request["code_value"];
            string strCodeDesc = context.Request["code_desc"];
            string strCode1 = context.Request["code1"];
            string strCode2 = context.Request["code2"];
            string strCode3 = context.Request["code3"];
            string strCode4 = context.Request["code4"];
            string strCode5 = context.Request["code5"];
            string strUser = context.Request["user"];

            string sql = string.Empty;
            //string sql = "select count(1) from hcode_line where code_cate = '{0}' and code_value='{1}';";
            //sql = string.Format(sql, strCodeCate, strCodeValue);
            //int count = db.ExecuteScalar(sql);
            if (!string.IsNullOrEmpty(strID))
            {
                sql = @"update hcode_line 
                            set code_value='{0}',code_desc='{1}',code1='{2}', code2='{3}', code3='{4}', code4='{5}', 
                                code5='{6}' ,last_updated_date=getdate(),last_updated_by='{7}'
                                where id={8};";
                sql = string.Format(sql, strCodeValue, strCodeDesc, strCode1, strCode2, strCode3, strCode4, strCode5, strUser, strID);
            }
            else
            {
                sql = @"insert into hcode_line (code_cate,code_value,code_desc,code1,code2,code3,code4,code5,creation_date,created_by) 
                             values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',getdate(),'{8}');";
                sql = string.Format(sql, strCodeCate, strCodeValue, strCodeDesc, strCode1, strCode2, strCode3, strCode4, strCode5, strUser);
            }

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

        private void DeleteCode(HttpContext context)
        {
            var id = context.Request["id"];

            string sql = "delete from hcode where id='" + id + "'";
            Result rs = new Result();
            DbHelper db = new DbHelper();
            if (db.ExecuteNonQuery(sql) == 1)
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

        private void DeleteLines(HttpContext context)
        {
            var id = context.Request["id"];

            string sql = "delete from hcode_line where id='" + id + "'";
            Result rs = new Result();
            DbHelper db = new DbHelper();
            if (db.ExecuteNonQuery(sql) == 1)
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}