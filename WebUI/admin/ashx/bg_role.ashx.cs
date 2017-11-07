using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZGZY.WebUI.admin.ashx
{
    /// <summary>
    /// 权限表操作
    /// </summary>
    public class bg_role : IHttpHandler
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
                        context.Response.Write(new ZGZY.BLL.Role().GetAllRole("1=1"));
                        break;
                    case "search":
                        string strWhere = "1=1";
                        string sort = context.Request.Params["sort"];  //排序列
                        string order = context.Request.Params["order"];  //排序方式 asc或者desc
                        int pageindex = int.Parse(context.Request.Params["page"]);
                        int pagesize = int.Parse(context.Request.Params["rows"]);

                        int totalCount;   //输出参数
                        string strJson = new ZGZY.BLL.Role().GetPager("tbRole", "Id,RoleName,AddDate,ModifyDate,Description", sort + " " + order, pagesize, pageindex, strWhere, out totalCount);
                        context.Response.Write("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");
                        userOperateLog.OperateInfo = "查询角色";
                        userOperateLog.IfSuccess = true;
                        userOperateLog.Description = "查询条件：" + strWhere + " 排序：" + sort + " " + order + " 页码/每页大小：" + pageindex + " " + pagesize;
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "searchRoleUser":
                        int roleUserId = int.Parse(context.Request.Params["roleId"]);
                        string sortRoleUser = context.Request.Params["sort"];  //排序列
                        string orderRoleUser = context.Request.Params["order"];  //排序方式 asc或者desc
                        int pageindexRoleUser = int.Parse(context.Request.Params["page"]);
                        int pagesizeRoleUser = int.Parse(context.Request.Params["rows"]);

                        string strJsonRoleUser = new ZGZY.BLL.Role().GetPagerRoleUser(roleUserId, sortRoleUser + " " + orderRoleUser, pagesizeRoleUser, pageindexRoleUser);
                        context.Response.Write(strJsonRoleUser);
                        userOperateLog.OperateInfo = "查询角色用户";
                        userOperateLog.IfSuccess = true;
                        userOperateLog.Description = "查询角色Id：" + roleUserId + " 排序：" + sortRoleUser + " " + orderRoleUser + " 页码/每页大小：" + pageindexRoleUser + " " + pagesizeRoleUser;
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "add":
                        if (user != null && new ZGZY.BLL.Authority().IfAuthority("role", "add", user.Id))
                        {
                            string ui_role_rolename_add = context.Request.Params["ui_role_rolename_add"] ?? "";
                            string ui_role_description_add = context.Request.Params["ui_role_description_add"] ?? "";

                            ZGZY.Model.Role roleAdd = new Model.Role();
                            roleAdd.RoleName = ui_role_rolename_add;
                            roleAdd.Description = ui_role_description_add.Trim();

                            int roleId = new ZGZY.BLL.Role().AddRole(roleAdd);
                            if (roleId > 0)
                            {
                                userOperateLog.OperateInfo = "添加角色";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "添加成功，角色主键：" + roleId;
                                context.Response.Write("{\"msg\":\"添加成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "添加角色";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "添加失败";
                                context.Response.Write("{\"msg\":\"添加失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "添加角色";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "edit":
                        if (user != null && new ZGZY.BLL.Authority().IfAuthority("role", "edit", user.Id))
                        {
                            int id = Convert.ToInt32(context.Request.Params["id"]);
                            string originalName = context.Request.Params["originalName"] ?? "";
                            string ui_role_rolename_edit = context.Request.Params["ui_role_rolename_edit"] ?? "";
                            string ui_role_description_edit = context.Request.Params["ui_role_description_edit"] ?? "";

                            ZGZY.Model.Role roleEdit = new Model.Role();
                            roleEdit.Id = id;
                            roleEdit.RoleName = ui_role_rolename_edit;
                            roleEdit.Description = ui_role_description_edit.Trim();

                            if (new ZGZY.BLL.Role().EditRole(roleEdit, originalName))
                            {
                                userOperateLog.OperateInfo = "修改角色";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "修改成功，角色主键：" + roleEdit.Id;
                                context.Response.Write("{\"msg\":\"修改成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "修改角色";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "修改失败";
                                context.Response.Write("{\"msg\":\"修改失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "修改角色";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "delete":
                        if (user != null && new ZGZY.BLL.Authority().IfAuthority("role", "delete", user.Id))
                        {
                            int id = Convert.ToInt32(context.Request.Params["id"]);
                            if (new ZGZY.BLL.Role().DeleteRole(id))
                            {
                                userOperateLog.OperateInfo = "删除角色";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "删除成功，角色主键：" + id;
                                context.Response.Write("{\"msg\":\"删除成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "删除角色";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "删除失败";
                                context.Response.Write("{\"msg\":\"删除失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "删除角色";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "authorize":
                        if (user != null && new ZGZY.BLL.Authority().IfAuthority("role", "authorize", user.Id))
                        {
                            int roleId = Convert.ToInt32(context.Request.Params["roleId"]);    //要授权的角色id
                            string menuButtonId = context.Request.Params["menuButtonId"].Trim(',');   //具体的菜单和按钮权限
                            if (new ZGZY.BLL.Role().Authorize(roleId, menuButtonId))
                            {
                                userOperateLog.OperateInfo = "角色授权";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "授权成功，菜单/按钮Id：" + menuButtonId;
                                context.Response.Write("{\"msg\":\"授权成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "角色授权";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "授权失败";
                                context.Response.Write("{\"msg\":\"授权失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "角色授权";
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
                userOperateLog.OperateInfo = "角色功能异常";
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