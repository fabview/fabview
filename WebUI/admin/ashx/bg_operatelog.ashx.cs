using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZGZY.WebUI.admin.ashx
{
    /// <summary>
    /// 用户操作记录表操作
    /// </summary>
    public class bg_operatelog : IHttpHandler
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

                        string ui_operatelog_username = context.Request.Params["ui_operatelog_username"] ?? "";
                        string ui_operatelog_userip = context.Request.Params["ui_operatelog_userip"] ?? "";
                        string ui_operatelog_info = context.Request.Params["ui_operatelog_info"] ?? "";
                        string ui_operatelog_description = context.Request.Params["ui_operatelog_description"] ?? "";
                        string ui_operatelog_success = context.Request.Params["ui_operatelog_success"] ?? "";
                        string ui_operatelog_operatedatestart = context.Request.Params["ui_operatelog_operatedatestart"] ?? "";
                        string ui_operatelog_operatedateend = context.Request.Params["ui_operatelog_operatedateend"] ?? "";

                        if (ui_operatelog_username.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_operatelog_username))   //防止sql注入
                            strWhere += string.Format(" and UserName like '%{0}%'", ui_operatelog_username.Trim());
                        if (ui_operatelog_userip.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_operatelog_userip))
                            strWhere += string.Format(" and UserIp like '%{0}%'", ui_operatelog_userip.Trim());
                        if (ui_operatelog_info.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_operatelog_info))
                            strWhere += string.Format(" and OperateInfo like '%{0}%'", ui_operatelog_info.Trim());
                        if (ui_operatelog_description.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_operatelog_description))
                            strWhere += string.Format(" and Description like '%{0}%'", ui_operatelog_description.Trim());
                        if (ui_operatelog_success.Trim() != "select" && ui_operatelog_success.Trim() != "")
                            strWhere += " and IfSuccess = '" + ui_operatelog_success.Trim() + "'";
                        if (ui_operatelog_operatedatestart.Trim() != "")
                            strWhere += " and OperateDate > '" + ui_operatelog_operatedatestart.Trim() + "'";
                        if (ui_operatelog_operatedateend.Trim() != "")
                            strWhere += " and OperateDate < '" + ui_operatelog_operatedateend.Trim() + "'";

                        userOperateLog.OperateInfo = "查询操作日志";
                        userOperateLog.IfSuccess = true;
                        userOperateLog.Description = "查询条件：" + strWhere + " 排序：" + sort + " " + order + " 页码/每页大小：" + pageindex + " " + pagesize;
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);

                        int totalCount;   //输出参数
                        string strJson = new ZGZY.BLL.UserOperateLog().GetPager("tbUserOperateLog", "Id,UserName,UserIp,OperateInfo,Description,IfSuccess,OperateDate", sort + " " + order, pagesize, pageindex, strWhere, out totalCount);
                        context.Response.Write("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");
                        break;
                    default:
                        context.Response.Write("{\"msg\":\"参数错误！\",\"success\":false}");
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"msg\":\"" + ZGZY.Common.JsonHelper.StringFilter(ex.Message) + "\",\"success\":false}");
                userOperateLog.OperateInfo = "操作日志功能异常";
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