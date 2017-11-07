using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZGZY.WebUI.admin.ashx
{
    /// <summary>
    /// 登陆日志表操作
    /// </summary>
    public class bg_loginlog : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string action = context.Request.Params["action"];
            ZGZY.Model.UserOperateLog userOperateLog = null;   //操作日志对象
            try
            {
                ZGZY.Model.User user = ZGZY.Common.UserHelper.GetUser(context);   //获取cookie里的用户对象
                userOperateLog = new Model.UserOperateLog();
                userOperateLog.UserIp = context.Request.UserHostAddress;
                userOperateLog.UserName = user.UserId;

                switch (action)
                {
                    case "search":
                        string strWhere = "1=1";
                        string sort = context.Request.Params["sort"];  //排序列
                        string order = context.Request.Params["order"];  //排序方式 asc或者desc
                        int pageindex = int.Parse(context.Request.Params["page"]);
                        int pagesize = int.Parse(context.Request.Params["rows"]);

                        string ui_loginlog_username = context.Request.Params["ui_loginlog_username"] ?? "";
                        string ui_loginlog_userip = context.Request.Params["ui_loginlog_userip"] ?? "";
                        string ui_loginlog_city = context.Request.Params["ui_loginlog_city"] ?? "";
                        string ui_loginlog_success = context.Request.Params["ui_loginlog_success"] ?? "";
                        string ui_loginlog_logindatestart = context.Request.Params["ui_loginlog_logindatestart"] ?? "";
                        string ui_loginlog_logindateend = context.Request.Params["ui_loginlog_logindateend"] ?? "";

                        if (ui_loginlog_username.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_loginlog_username))   //防止sql注入
                            strWhere += string.Format(" and UserName like '%{0}%'", ui_loginlog_username.Trim());
                        if (ui_loginlog_userip.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_loginlog_userip))
                            strWhere += string.Format(" and UserIp like '%{0}%'", ui_loginlog_userip.Trim());
                        if (ui_loginlog_city.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_loginlog_city))
                            strWhere += string.Format(" and City like '%{0}%'", ui_loginlog_city.Trim());
                        if (ui_loginlog_success.Trim() != "select" && ui_loginlog_success.Trim() != "")
                            strWhere += " and Success = '" + ui_loginlog_success.Trim() + "'";
                        if (ui_loginlog_logindatestart.Trim() != "")
                            strWhere += " and LoginDate > '" + ui_loginlog_logindatestart.Trim() + "'";
                        if (ui_loginlog_logindateend.Trim() != "")
                            strWhere += " and LoginDate < '" + ui_loginlog_logindateend.Trim() + "'";

                        string strJson = new ZGZY.BLL.LoginLog().GetPager(strWhere, sort, order, pageindex, pagesize);
                        context.Response.Write(strJson);
                        userOperateLog.OperateInfo = "查询登陆日志";
                        userOperateLog.IfSuccess = true;
                        userOperateLog.Description = "查询条件：" + strWhere + " 排序：" + sort + " " + order + " 页码/每页大小：" + pageindex + " " + pagesize;
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "export":  //导出

                        break;
                    default:
                        context.Response.Write("{\"msg\":\"参数错误！\",\"success\":false}");
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"msg\":\"" + ZGZY.Common.JsonHelper.StringFilter(ex.Message) + "\",\"success\":false}");
                userOperateLog.OperateInfo = "登陆日志功能异常";
                userOperateLog.IfSuccess = false;
                userOperateLog.Description = ZGZY.Common.JsonHelper.StringFilter(ex.Message);
                ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
            }
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