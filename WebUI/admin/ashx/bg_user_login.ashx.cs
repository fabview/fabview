using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Script.Serialization;

namespace ZGZY.WebUI.admin.ashx
{
    /// <summary>
    /// 用户登录退出
    /// </summary>
    public class bg_user_login : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string action = context.Request.Params["action"];

            try
            {
                switch (action)
                {
                    case "getuser":
                        if (context.Request.IsAuthenticated)
                        {
                            FormsIdentity id = (FormsIdentity)context.User.Identity;
                            FormsAuthenticationTicket tickets = id.Ticket;

                            //反序列化获取票证里的用户对象（这个用户对象是cookie里保存的，不一定是数据库里最新的用户状态）
                            ZGZY.Model.User userFromCookie = new JavaScriptSerializer().Deserialize<ZGZY.Model.User>(tickets.UserData);
                            //执行登录操作（获取数据库里最新的用户对象）
                            ZGZY.Model.User userFromDB = new ZGZY.BLL.User().UserLogin(userFromCookie.UserId, userFromCookie.UserPwd);

                            if (userFromDB == null)   //修改了用户名或密码
                            {
                                FormsAuthentication.SignOut();   //干掉cookie
                                context.Response.Write("{\"msg\":\"用户名或密码错误！\",\"success\":false}");
                            }
                            else if (!userFromDB.IsAble)   //管理员禁用了这个账户
                            {
                                FormsAuthentication.SignOut();   //干掉cookie
                                context.Response.Write("{\"msg\":\"用户已被禁用！\",\"success\":false}");
                            }
                            else if (userFromCookie.IfChangePwd != userFromDB.IfChangePwd || userFromCookie.UserName != userFromDB.UserName)   //如果这两个字段修改了需要重新生成cookie
                            {
                                FormsAuthentication.SignOut();
                                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                                (
                                    2,
                                    userFromDB.UserId,
                                    DateTime.Now,
                                    tickets.Expiration,
                                    false,
                                    new JavaScriptSerializer().Serialize(userFromDB)  //序列化新的用户对象
                                );
                                string encTicket = FormsAuthentication.Encrypt(ticket);   //加密
                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                                if (ticket.Expiration != new DateTime(9999, 12, 31))    //不是默认时间才设置过期时间，否则会话cookie
                                    cookie.Expires = tickets.Expiration;
                                context.Response.Cookies.Add(cookie);  //写入cookie

                                //输出新对象
                                context.Response.Write("{\"msg\":" + new JavaScriptSerializer().Serialize(userFromDB) + ",\"success\":true}");
                            }
                            else
                            {
                                context.Response.Write("{\"msg\":" + tickets.UserData + ",\"success\":true}");
                            }
                        }
                        else
                            context.Response.Write("{\"msg\":\"nocookie\",\"success\":false}");
                        break;
                    case "iflogin":
                        //System.Threading.Thread.Sleep(5000);
                        if (context.Request.IsAuthenticated)
                        {
                            FormsIdentity id = (FormsIdentity)context.User.Identity;
                            FormsAuthenticationTicket tickets = id.Ticket;

                            //获取票证里序列化的用户对象（反序列化）
                            ZGZY.Model.User userCheck = new JavaScriptSerializer().Deserialize<ZGZY.Model.User>(tickets.UserData);
                            //执行登录操作
                            ZGZY.Model.User userReLogin = new ZGZY.BLL.User().UserLogin(userCheck.UserId, userCheck.UserPwd);

                            if (userReLogin == null)
                            {
                                FormsAuthentication.SignOut();
                                context.Response.Write("{\"msg\":\"用户名或密码错误！\",\"success\":false}");
                            }
                            else if (!userReLogin.IsAble)
                            {
                                FormsAuthentication.SignOut();
                                context.Response.Write("{\"msg\":\"用户已被禁用！\",\"success\":false}");
                            }
                            else
                            {
                                //记录登录日志
                                ZGZY.Model.LoginLog loginInfo = new Model.LoginLog();
                                loginInfo.UserIp = context.Request.UserHostAddress;
                                loginInfo.City = context.Request.Params["city"] ?? "未知";   //访问者所处城市
                                loginInfo.UserName = context.User.Identity.Name;
                                loginInfo.Success = true;
                                new ZGZY.BLL.LoginLog().WriteLoginLog(loginInfo);

                                context.Response.Write("{\"msg\":\"已登录过，正在跳转！\",\"success\":true}");
                            }
                        }
                        else
                            context.Response.Write("{\"msg\":\"nocookie\",\"success\":false}");
                        break;
                    case "login":
                        //System.Threading.Thread.Sleep(5000);
                        string userIp = context.Request.UserHostAddress;
                        string city = context.Request.Params["city"] ?? "未知";
                        string remember = context.Request.Params["remember"] ?? "";   //记住密码天数
                        string name = context.Request.Params["loginName"];
                        string pwd = ZGZY.Common.Md5.GetMD5String(context.Request.Params["loginPwd"]);  //md5加密
                        DateTime? lastLoginTime;
                        if (new ZGZY.BLL.LoginLog().CheckLogin(userIp, out lastLoginTime) != null)
                        {
                            DateTime dtNextLogin = Convert.ToDateTime(lastLoginTime);
                            context.Response.Write("{\"msg\":\"密码错误次数达到5次，请在" + dtNextLogin.AddMinutes(30).ToShortTimeString() + "之后再登陆！\",\"success\":false}");
                        }
                        else
                        {
                            ZGZY.Model.LoginLog loginInfo = new Model.LoginLog();
                            loginInfo.UserName = name;
                            loginInfo.UserIp = userIp;
                            loginInfo.City = city;
                            ZGZY.Model.User currentUser = new ZGZY.BLL.User().UserLogin(name, pwd);
                            if (currentUser == null)
                            {
                                context.Response.Write("{\"msg\":\"用户名或密码错误！\",\"success\":false}");
                                loginInfo.Success = false;
                                new ZGZY.BLL.LoginLog().WriteLoginLog(loginInfo);
                            }
                            else if (currentUser.IsAble == false)
                            {
                                context.Response.Write("{\"msg\":\"用户已被禁用！\",\"success\":false}");
                                loginInfo.Success = false;
                                new ZGZY.BLL.LoginLog().WriteLoginLog(loginInfo);
                            }
                            else
                            {
                                //记录登录日志
                                loginInfo.Success = true;
                                new ZGZY.BLL.LoginLog().WriteLoginLog(loginInfo);
                                context.Response.Write("{\"msg\":\"登录成功！\",\"success\":true}");

                                DateTime dateCookieExpires;  //cookie有效期
                                switch (remember)
                                {
                                    case "notremember":
                                        dateCookieExpires = new DateTime(9999, 12, 31);   //默认时间
                                        break;
                                    case "oneday":
                                        dateCookieExpires = DateTime.Now.AddDays(1);
                                        break;
                                    case "sevenday":
                                        dateCookieExpires = DateTime.Now.AddDays(7);
                                        break;
                                    case "onemouth":
                                        dateCookieExpires = DateTime.Now.AddDays(30);
                                        break;
                                    case "oneyear":
                                        dateCookieExpires = DateTime.Now.AddDays(365);
                                        break;
                                    default:
                                        dateCookieExpires = new DateTime(9999, 12, 31);
                                        break;
                                }
                                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                                (
                                    2,
                                    currentUser.UserId,
                                    DateTime.Now,
                                    dateCookieExpires,
                                    false,
                                    new JavaScriptSerializer().Serialize(currentUser)  //序列化当前用户对象
                                );
                                string encTicket = FormsAuthentication.Encrypt(ticket);
                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                                if (dateCookieExpires != new DateTime(9999, 12, 31))    //不是默认时间才设置过期时间，否则会话cookie
                                    cookie.Expires = dateCookieExpires;
                                context.Response.Cookies.Add(cookie);
                            }
                        }
                        break;
                    case "logout":
                        FormsAuthentication.SignOut();
                        context.Response.Write("{\"msg\":\"退出成功！\",\"success\":true}");
                        break;
                    default:
                        context.Response.Write("{\"msg\":\"参数错误！\",\"success\":false}");
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"msg\":\"" + ZGZY.Common.JsonHelper.StringFilter(ex.Message) + "\",\"success\":false}");
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