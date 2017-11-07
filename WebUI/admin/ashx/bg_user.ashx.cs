using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Data;

namespace ZGZY.WebUI.admin.ashx
{
    /// <summary>
    /// 用户表操作
    /// </summary>
    public class bg_user : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string action = context.Request.Params["action"];
            ZGZY.Model.UserOperateLog userOperateLog = null;   //操作日志对象
            try
            {
                ZGZY.Model.User userFromCookie = ZGZY.Common.UserHelper.GetUser(context);   //获取cookie里的用户对象
                userOperateLog = new Model.UserOperateLog();
                userOperateLog.UserIp = context.Request.UserHostAddress;
                userOperateLog.UserName = userFromCookie.UserId;
                switch (action)
                {
                    case "firstlogin":
                        int ui_user_firstlogin_id = Convert.ToInt32(context.Request.Params["ui_user_firstlogin_id"]);
                        string ui_user_firstlogin_pwd = context.Request.Params["ui_user_firstlogin_pwd"] ?? "";
                        if (userFromCookie != null && userFromCookie.Id == ui_user_firstlogin_id)   //只能修改当前登录的用户
                        {
                            ZGZY.Model.User initUser = new Model.User();
                            initUser.Id = ui_user_firstlogin_id;
                            initUser.UserPwd = ZGZY.Common.Md5.GetMD5String(ui_user_firstlogin_pwd);   //加密
                            if (initUser.UserPwd != userFromCookie.UserPwd)
                            {
                                if (new ZGZY.BLL.User().InitUserPwd(initUser))
                                {
                                    //修改成功需要重写cookie，否则cookie里的密码不对下次经过bg_user_login.asxh里的getuser语句块就自动退出了
                                    FormsIdentity id = (FormsIdentity)context.User.Identity;
                                    FormsAuthenticationTicket ticketOld = id.Ticket;
                                    userFromCookie.UserPwd = initUser.UserPwd;   //赋值新密码，其他属性不变

                                    FormsAuthentication.SignOut();
                                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                                    (
                                        2,
                                        userFromCookie.UserId,
                                        DateTime.Now,
                                        ticketOld.Expiration,
                                        false,
                                        new JavaScriptSerializer().Serialize(userFromCookie)  //序列化新的用户对象
                                    );
                                    string encTicket = FormsAuthentication.Encrypt(ticket);
                                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                                    if (ticket.Expiration != new DateTime(9999, 12, 31))    //不是默认时间才设置过期时间，否则会话cookie
                                        cookie.Expires = ticketOld.Expiration;
                                    context.Response.Cookies.Add(cookie);

                                    userOperateLog.OperateInfo = "用户重置密码";
                                    userOperateLog.IfSuccess = true;
                                    userOperateLog.Description = "重置密码成功";
                                    context.Response.Write("{\"msg\":\"重置密码成功！\",\"success\":true}");
                                }
                                else
                                {
                                    userOperateLog.OperateInfo = "用户重置密码";
                                    userOperateLog.IfSuccess = false;
                                    userOperateLog.Description = "重置密码失败";
                                    context.Response.Write("{\"msg\":\"重置密码失败！\",\"success\":false}");
                                }
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "用户重置密码";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "重置密码失败，不能和默认密码一样";
                                context.Response.Write("{\"msg\":\"重置密码失败，不能和默认密码一样！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "用户重置密码";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "未知错误，重置密码失败";
                            context.Response.Write("{\"msg\":\"未知错误，重置密码失败！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "changepwd":
                        string ui_user_userchangepwd_originalpwd = context.Request.Params["ui_user_userchangepwd_originalpwd"] ?? "";
                        string ui_user_userchangepwd_newpwd = context.Request.Params["ui_user_userchangepwd_newpwd"] ?? "";

                        ZGZY.Model.User userChangePwd = new Model.User();
                        userChangePwd.Id = userFromCookie.Id;
                        userChangePwd.UserPwd = ZGZY.Common.Md5.GetMD5String(ui_user_userchangepwd_newpwd);   //md5加密

                        if (ZGZY.Common.Md5.GetMD5String(ui_user_userchangepwd_originalpwd) == userFromCookie.UserPwd)
                        {
                            if (new ZGZY.BLL.User().ChangePwd(userChangePwd))
                            {
                                FormsAuthentication.SignOut();    //这里如果不退出还得重写cookie
                                userOperateLog.OperateInfo = "用户修改密码";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "修改成功，用户主键：" + userChangePwd.Id;
                                context.Response.Write("{\"msg\":\"修改成功，正在跳转到登陆页面！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "用户修改密码";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "修改失败";
                                context.Response.Write("{\"msg\":\"修改失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "用户修改密码";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "原密码不正确";
                            context.Response.Write("{\"msg\":\"原密码不正确！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "getUserInfo":
                        context.Response.Write(new ZGZY.BLL.User().GetUserInfo(userFromCookie.Id));   //“我的信息”
                        break;
                    case "search":
                        string strWhere = "1=1";
                        string sort = context.Request.Params["sort"];  //排序列
                        string order = context.Request.Params["order"];  //排序方式 asc或者desc
                        int pageindex = int.Parse(context.Request.Params["page"]);
                        int pagesize = int.Parse(context.Request.Params["rows"]);

                        string ui_user_userid = context.Request.Params["ui_user_userid"] ?? "";
                        string ui_user_username = context.Request.Params["ui_user_username"] ?? "";
                        string ui_user_isable = context.Request.Params["ui_user_isable"] ?? "";
                        string ui_user_ifchangepwd = context.Request.Params["ui_user_ifchangepwd"] ?? "";
                        string ui_user_description = context.Request.Params["ui_user_description"] ?? "";
                        string ui_user_adddatestart = context.Request.Params["ui_user_adddatestart"] ?? "";
                        string ui_user_adddateend = context.Request.Params["ui_user_adddateend"] ?? "";

                        if (ui_user_userid.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_user_userid))   //防止sql注入
                            strWhere += string.Format(" and UserId like '%{0}%'", ui_user_userid.Trim());
                        if (ui_user_username.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_user_username))
                            strWhere += string.Format(" and UserName like '%{0}%'", ui_user_username.Trim());
                        if (ui_user_description.Trim() != "" && !ZGZY.Common.SqlInjection.GetString(ui_user_description))
                            strWhere += string.Format(" and Description like '%{0}%'", ui_user_description.Trim());
                        if (ui_user_isable.Trim() != "select" && ui_user_isable.Trim() != "")
                            strWhere += " and IsAble = '" + ui_user_isable.Trim() + "'";
                        if (ui_user_ifchangepwd.Trim() != "select" && ui_user_ifchangepwd.Trim() != "")
                            strWhere += " and IfChangePwd = '" + ui_user_ifchangepwd.Trim() + "'";
                        if (ui_user_adddatestart.Trim() != "")
                            strWhere += " and AddDate > '" + ui_user_adddatestart.Trim() + "'";
                        if (ui_user_adddateend.Trim() != "")
                            strWhere += " and AddDate < '" + ui_user_adddateend.Trim() + "'";

                        int totalCount;   //输出参数
                        string strJson = new ZGZY.BLL.User().GetPager("tbUser", "Id,UserId,UserName,IsAble,IfChangePwd,AddDate,Description", sort + " " + order, pagesize, pageindex, strWhere, out totalCount);
                        context.Response.Write("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");

                        userOperateLog.OperateInfo = "查询用户";
                        userOperateLog.IfSuccess = true;
                        userOperateLog.Description = "查询条件：" + strWhere + " 排序：" + sort + " " + order + " 页码/每页大小：" + pageindex + " " + pagesize;
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "add":
                        if (userFromCookie != null && new ZGZY.BLL.Authority().IfAuthority("user", "add", userFromCookie.Id))
                        {
                            string ui_user_userid_add = context.Request.Params["ui_user_userid_add"] ?? "";
                            string ui_user_username_add = context.Request.Params["ui_user_username_add"] ?? "";
                            bool ui_user_isable_add = context.Request.Params["ui_user_isable_add"] == null ? false : true;
                            bool ui_user_ifchangepwd_add = context.Request.Params["ui_user_ifchangepwd_add"] == null ? false : true;
                            string ui_user_description_add = context.Request.Params["ui_user_description_add"] ?? "";

                            ZGZY.Model.User userAdd = new Model.User();
                            userAdd.UserId = ui_user_userid_add.Trim();
                            userAdd.UserName = ui_user_username_add.Trim();
                            userAdd.UserPwd = ZGZY.Common.Md5.GetMD5String("123");   //md5加密
                            userAdd.IsAble = ui_user_isable_add;
                            userAdd.IfChangePwd = ui_user_ifchangepwd_add;
                            userAdd.Description = ui_user_description_add.Trim();

                            int userId = new ZGZY.BLL.User().AddUser(userAdd);
                            if (userId > 0)
                            {
                                userOperateLog.OperateInfo = "添加用户";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "添加成功，用户主键：" + userId;
                                context.Response.Write("{\"msg\":\"添加成功！默认密码是【123】\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "添加用户";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "添加失败";
                                context.Response.Write("{\"msg\":\"添加失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "添加用户";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "edit":
                        if (userFromCookie != null && new ZGZY.BLL.Authority().IfAuthority("user", "edit", userFromCookie.Id))
                        {
                            int id = Convert.ToInt32(context.Request.Params["id"]);
                            string originalName = context.Request.Params["originalName"] ?? "";
                            string ui_user_userid_edit = context.Request.Params["ui_user_userid_edit"] ?? "";
                            string ui_user_username_edit = context.Request.Params["ui_user_username_edit"] ?? "";
                            bool ui_user_isable_edit = context.Request.Params["ui_user_isable_edit"] == null ? false : true;
                            bool ui_user_ifchangepwd_edit = context.Request.Params["ui_user_ifchangepwd_edit"] == null ? false : true;
                            string ui_user_description_edit = context.Request.Params["ui_user_description_edit"] ?? "";

                            ZGZY.Model.User userEdit = new Model.User();
                            userEdit.Id = id;
                            userEdit.UserId = ui_user_userid_edit.Trim();
                            userEdit.UserName = ui_user_username_edit.Trim();
                            userEdit.IsAble = ui_user_isable_edit;
                            userEdit.IfChangePwd = ui_user_ifchangepwd_edit;
                            userEdit.Description = ui_user_description_edit.Trim();

                            if (new ZGZY.BLL.User().EditUser(userEdit, originalName))
                            {
                                userOperateLog.OperateInfo = "修改用户";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "修改成功，用户主键：" + userEdit.Id;
                                context.Response.Write("{\"msg\":\"修改成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "修改用户";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "修改失败";
                                context.Response.Write("{\"msg\":\"修改失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "修改用户";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "delete":
                        if (userFromCookie != null && new ZGZY.BLL.Authority().IfAuthority("user", "delete", userFromCookie.Id))
                        {
                            string ids = context.Request.Params["id"].Trim(',');
                            if (new ZGZY.BLL.User().DeleteUser(ids))
                            {
                                userOperateLog.OperateInfo = "删除用户";
                                userOperateLog.IfSuccess = true;
                                userOperateLog.Description = "删除成功，用户主键：" + ids;
                                context.Response.Write("{\"msg\":\"删除成功！\",\"success\":true}");
                            }
                            else
                            {
                                userOperateLog.OperateInfo = "删除用户";
                                userOperateLog.IfSuccess = false;
                                userOperateLog.Description = "删除失败";
                                context.Response.Write("{\"msg\":\"删除失败！\",\"success\":false}");
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "删除用户";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "setdep":
                        if (userFromCookie != null && new ZGZY.BLL.Authority().IfAuthority("user", "setdepartment", userFromCookie.Id))
                        {
                            string ui_user_setdep_userid = context.Request.Params["ui_user_setdep_userid"] ?? "";
                            string ui_user_setdep_dep = context.Request.Params["ui_user_setdep_dep"] ?? "";

                            if (ui_user_setdep_userid.IndexOf(",") == -1)  //单个用户设置部门
                            {
                                if (ui_user_setdep_userid != "" && new BLL.UserDepartment().SetDepartmentSingle(Convert.ToInt32(ui_user_setdep_userid), ui_user_setdep_dep))
                                {
                                    userOperateLog.OperateInfo = "设置用户部门";
                                    userOperateLog.IfSuccess = true;
                                    userOperateLog.Description = "设置成功，用户主键：" + ui_user_setdep_userid + " 部门主键：" + ui_user_setdep_dep;
                                    context.Response.Write("{\"msg\":\"设置成功！\",\"success\":true}");
                                }
                                else
                                {
                                    userOperateLog.OperateInfo = "设置用户部门";
                                    userOperateLog.IfSuccess = false;
                                    userOperateLog.Description = "设置失败，用户主键：" + ui_user_setdep_userid + " 部门主键：" + ui_user_setdep_dep;
                                    context.Response.Write("{\"msg\":\"设置失败！\",\"success\":true}");
                                }
                            }
                            else   //批量设置用户部门
                            {
                                if (ui_user_setdep_userid != "" && new BLL.UserDepartment().SetDepartmentBatch(ui_user_setdep_userid, ui_user_setdep_dep))
                                {
                                    userOperateLog.OperateInfo = "批量设置用户部门";
                                    userOperateLog.IfSuccess = true;
                                    userOperateLog.Description = "设置成功，用户主键：" + ui_user_setdep_userid + " 部门主键：" + ui_user_setdep_dep;
                                    context.Response.Write("{\"msg\":\"设置成功！\",\"success\":true}");
                                }
                                else
                                {
                                    userOperateLog.OperateInfo = "批量设置用户部门";
                                    userOperateLog.IfSuccess = false;
                                    userOperateLog.Description = "设置失败，用户主键：" + ui_user_setdep_userid + " 部门主键：" + ui_user_setdep_dep;
                                    context.Response.Write("{\"msg\":\"设置失败！\",\"success\":true}");
                                }
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "设置用户部门";
                            userOperateLog.IfSuccess = false;
                            userOperateLog.Description = "无权限，请联系管理员";
                            context.Response.Write("{\"msg\":\"无权限，请联系管理员！\",\"success\":false}");
                        }
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "setrole":
                        if (userFromCookie != null && new ZGZY.BLL.Authority().IfAuthority("user", "setrole", userFromCookie.Id))
                        {
                            string ui_user_setrole_userid = context.Request.Params["ui_user_setrole_userid"] ?? "";  //用户id，可能是多个
                            string ui_user_setrole_role = context.Request.Params["ui_user_setrole_role"] ?? "";  //角色id，可能是多个

                            if (ui_user_setrole_userid.IndexOf(",") == -1)  //单个用户分配角色
                            {
                                if (ui_user_setrole_userid != "" && new BLL.UserRole().SetRoleSingle(Convert.ToInt32(ui_user_setrole_userid), ui_user_setrole_role))
                                {
                                    userOperateLog.OperateInfo = "设置用户角色";
                                    userOperateLog.IfSuccess = true;
                                    userOperateLog.Description = "设置成功，用户主键：" + ui_user_setrole_userid + " 角色主键：" + ui_user_setrole_role;
                                    context.Response.Write("{\"msg\":\"设置成功！\",\"success\":true}");
                                }
                                else
                                {
                                    userOperateLog.OperateInfo = "设置用户角色";
                                    userOperateLog.IfSuccess = false;
                                    userOperateLog.Description = "设置失败，用户主键：" + ui_user_setrole_userid + " 角色主键：" + ui_user_setrole_role;
                                    context.Response.Write("{\"msg\":\"设置失败！\",\"success\":true}");
                                }
                            }
                            else   //批量设置用户角色
                            {
                                if (ui_user_setrole_userid != "" && new BLL.UserRole().SetRoleBatch(ui_user_setrole_userid, ui_user_setrole_role))
                                {
                                    userOperateLog.OperateInfo = "批量设置用户角色";
                                    userOperateLog.IfSuccess = true;
                                    userOperateLog.Description = "设置成功，用户主键：" + ui_user_setrole_userid + " 角色主键：" + ui_user_setrole_role;
                                    context.Response.Write("{\"msg\":\"设置成功！\",\"success\":true}");
                                }
                                else
                                {
                                    userOperateLog.OperateInfo = "批量设置用户角色";
                                    userOperateLog.IfSuccess = false;
                                    userOperateLog.Description = "设置失败，用户主键：" + ui_user_setrole_userid + " 角色主键：" + ui_user_setrole_role;
                                    context.Response.Write("{\"msg\":\"设置失败！\",\"success\":true}");
                                }
                            }
                        }
                        else
                        {
                            userOperateLog.OperateInfo = "设置用户角色";
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
                userOperateLog.OperateInfo = "用户功能异常";
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