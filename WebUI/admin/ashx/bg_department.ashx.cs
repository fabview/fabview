using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ZGZY.WebUI.admin.ashx
{
    /// <summary>
    /// 部门表操作
    /// </summary>
    public class bg_department : IHttpHandler
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
                    case "getall":
                        context.Response.Write(new ZGZY.BLL.Department().GetAllDepartment("1=1"));
                        break;
                    case "search":
                        string strJson = new ZGZY.BLL.Department().GetAllDepartment(null);
                        context.Response.Write(strJson);
                        userOperateLog.OperateInfo = "查询部门";
                        userOperateLog.IfSuccess = true;
                        userOperateLog.Description = "查询条件：1=1";
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "searchDepartmentUser":
                        string userDepartmentIds = context.Request.Params["departmentId"];
                        string sortDepartmentUser = context.Request.Params["sort"];  //排序列
                        string orderDepartmentUser = context.Request.Params["order"];  //排序方式 asc或者desc
                        int pageindexDepartmentUser = int.Parse(context.Request.Params["page"]);
                        int pagesizeDepartmentUser = int.Parse(context.Request.Params["rows"]);

                        string strJsonDepartmentUser = new ZGZY.BLL.Department().GetPagerDepartmentUser(userDepartmentIds, sortDepartmentUser + " " + orderDepartmentUser, pagesizeDepartmentUser, pageindexDepartmentUser);
                        context.Response.Write(strJsonDepartmentUser);
                        userOperateLog.OperateInfo = "查询部门用户";
                        userOperateLog.IfSuccess = true;
                        userOperateLog.Description = "查询部门Id：" + userDepartmentIds + " 排序：" + sortDepartmentUser + " " + orderDepartmentUser + " 页码/每页大小：" + pageindexDepartmentUser + " " + pagesizeDepartmentUser;
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "add":
                        if (user != null && new ZGZY.BLL.Authority().IfAuthority("department", "add", user.Id))
                        {
                            ZGZY.Model.Department departmentAdd = new Model.Department();
                            departmentAdd.DepartmentName = context.Request.Params["ui_department_departmentname_add"] ?? "";
                            departmentAdd.Sort = Convert.ToInt32(context.Request.Params["ui_department_sort_add"]);
                            if (context.Request.Params["ui_department_parentid_add"] != null && context.Request.Params["ui_department_parentid_add"] != "")
                                departmentAdd.ParentId = Convert.ToInt32(context.Request.Params["ui_department_parentid_add"]);
                            else
                                departmentAdd.ParentId = 0;   //根节点

                            int departmentId = new ZGZY.BLL.Department().AddDepartment(departmentAdd);
                            if (departmentId > 0)
                            {
                                userOperateLog.OperateInfo = "添加部门";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "添加成功，部门主键：" + departmentId;
                                context.Response.Write("{\"msg\":\"添加成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "添加部门";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "添加失败";
                                context.Response.Write("{\"msg\":\"添加失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "添加部门";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":true}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "edit":
                        if (user != null && new ZGZY.BLL.Authority().IfAuthority("department", "edit", user.Id))
                        {
                            ZGZY.Model.Department departmentEdit = new Model.Department();
                            departmentEdit.Id = Convert.ToInt32(context.Request.Params["id"]);
                            departmentEdit.DepartmentName = context.Request.Params["ui_department_departmentname_edit"] ?? "";
                            departmentEdit.Sort = Convert.ToInt32(context.Request.Params["ui_department_sort_edit"]);

                            bool result = new ZGZY.BLL.Department().EditDepartment(departmentEdit);
                            if (result)
                            {
                                userOperateLog.OperateInfo = "修改部门";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "修改成功，部门主键：" + departmentEdit.Id;
                                context.Response.Write("{\"msg\":\"修改成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "修改部门";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "修改失败，部门主键：" + departmentEdit.Id;
                                context.Response.Write("{\"msg\":\"修改失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "修改部门";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "delete":
                        if (user != null && new ZGZY.BLL.Authority().IfAuthority("department", "delete", user.Id))
                        {
                            string departmentIds = context.Request.Params["id"];
                            if (new ZGZY.BLL.Department().DeleteDepartment(departmentIds))
                            {
                                userOperateLog.OperateInfo = "删除部门";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "删除成功，部门主键：" + departmentIds;
                                context.Response.Write("{\"msg\":\"删除成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "删除部门";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "删除失败，部门主键：" + departmentIds;
                                context.Response.Write("{\"msg\":\"删除失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "删除部门";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    default:
                        context.Response.Write("{\"msg\":\"参数错误！\",\"success\":false}");
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"msg\":\"" + ZGZY.Common.JsonHelper.StringFilter(ex.Message) + "\",\"success\":false}");
                userOperateLog.OperateInfo = "部门功能异常";
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