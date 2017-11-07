using System;
using System.Web;
using FabView.Utility;
using Newtonsoft.Json;
using System.Text;

namespace ZGZY.WebUI.admin.ashx {
    /// <summary>
    /// 后台导航树
    /// </summary>
    public class bg_menu : IHttpHandler {
        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "application/json";
            string action = context.Request.Params["action"];
            ZGZY.Model.UserOperateLog userOperateLog = null;   //操作日志对象
            try {
                ZGZY.Model.User user = ZGZY.Common.UserHelper.GetUser(context);   //获取cookie里的用户对象
                userOperateLog = new Model.UserOperateLog();
                userOperateLog.UserIp = context.Request.UserHostAddress;
                userOperateLog.UserName = user.UserId;

                switch (action) {
                    case "getUserMenu":  //获取特定用户能看到的菜单（左侧树）
                        context.Response.Write(new ZGZY.BLL.Menu().GetUserMenu(user.Id));
                        break;
                    case "getAllMenu":   //根据角色id获取此角色有的权限（设置角色时自动勾选已经有的按钮权限）
                        int roleid = Convert.ToInt32(context.Request.Params["roleid"]);  //角色id
                        context.Response.Write(new ZGZY.BLL.Menu().GetAllMenu(roleid));
                        break;
                    case "getMyAuthority":  //前台根据用户名查“我的权限”
                        context.Response.Write(new ZGZY.BLL.Menu().GetMyAuthority(user.Id));
                        userOperateLog.OperateInfo = "查询我的信息";
                        userOperateLog.IfSuccess = true;
                        userOperateLog.Description = "查询我的信息";
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "search":
                        string strWhere = "1=1";
                        string sort = context.Request.Params["sort"] == null ? "Id" : context.Request.Params["sort"];  //排序列
                        string order = context.Request.Params["order"] == null ? "asc" : context.Request.Params["order"];  //排序方式 asc或者desc
                        int pageindex = int.Parse(context.Request.Params["page"]);
                        int pagesize = int.Parse(context.Request.Params["rows"]);

                        int totalCount;   //输出参数
                        string strJson = "";    //输出结果
                        if (order.IndexOf(',') != -1)   //如果有","就是多列排序（不能拿列判断，列名中间可能有","符号）
                        {
                            //多列排序：
                            //sort：ParentId,Sort,AddDate
                            //order：asc,desc,asc
                            string sortMulti = "";  //拼接排序条件，例：ParentId desc,Sort asc
                            string[] sortArray = sort.Split(',');   //列名中间有","符号，这里也要出错。正常不会有
                            string[] orderArray = order.Split(',');
                            for (int i = 0; i < sortArray.Length; i++) {
                                sortMulti += sortArray[i] + " " + orderArray[i] + ",";
                            }
                            strJson = new ZGZY.BLL.Menu().GetPager("tbMenu", "Id,Name,ParentId,Code,LinkAddress,Icon,Sort,AddDate", sortMulti.Trim(','), pagesize, pageindex, strWhere, out totalCount);
                            userOperateLog.Description = "查询条件：" + strWhere + " 排序：" + sortMulti.Trim(',') + " 页码/每页大小：" + pageindex + " " + pagesize;
                        } else {
                            strJson = new ZGZY.BLL.Menu().GetPager("tbMenu", "Id,Name,ParentId,Code,LinkAddress,Icon,Sort,AddDate", sort + " " + order, pagesize, pageindex, strWhere, out totalCount);
                            userOperateLog.Description = "查询条件：" + strWhere + " 排序：" + sort + " " + order + " 页码/每页大小：" + pageindex + " " + pagesize;
                        }

                        context.Response.Write("{\"total\": " + totalCount.ToString() + ",\"rows\":" + strJson + "}");
                        userOperateLog.OperateInfo = "查询菜单";
                        userOperateLog.IfSuccess = true;
                        ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
                        break;
                    case "add":
                        DbHelper db = new DbHelper();
                        Result rs = new Result();
                        string sqlMenu = @"insert into tbMenu(name,parentid,code,linkaddress,adddate)
                                    values('{0}',{1},'{2}','{3}',getdate());";

                        string formType = context.Request["formType"];
                        string formName = context.Request["formName"];
                        string mainFormID = context.Request["mainFormID"];
                        string htmlPath = context.Request["htmlPath"];
                        string sqlExist = "select count(1) from tbMenu where name='" + formName + "';";
                        int count = db.ExecuteScalar(sqlExist);
                        if (count > 0) {
                            rs.status = 0;
                            rs.msg = "该界面名称已经存在，请检查。";
                            context.Response.Write("{\"msg\":\"该界面名称已经存在，请检查。\",\"success\":false}");
                            return;
                        } else {

                            if (formType == "mainForm") {
                                sqlMenu = string.Format(sqlMenu, formName, "0", "", "");
                            } else {
                                sqlMenu = string.Format(sqlMenu, formName, mainFormID, "Function", htmlPath);
                            }

                            string sqlMenuButton = "insert into tbMenuButton select id, 1 from tbmenu where name='" + formName + "';";

                            if (db.ExecuteNonQuery(sqlMenu) == 1) {
                                if (db.ExecuteNonQuery(sqlMenuButton) == 1) {
                                    rs.status = 1;
                                    rs.msg = "保存成功！";
                                }
                            } else {
                                rs.status = 0;
                                rs.msg = "服务器繁忙，请稍后再试！";
                            }
                        }

                        rs.item = "[]";
                        string json = JsonConvert.SerializeObject(rs);

                        context.Response.Clear();
                        context.Response.ContentEncoding = Encoding.UTF8;
                        context.Response.ContentType = "application/json";
                        context.Response.Write(json);
                        context.Response.Flush();
                        context.Response.End();

                        break;
                    default:
                        context.Response.Write("{\"result\":\"参数错误！\",\"success\":false}");
                        break;
                }
            } catch (Exception ex) {
                context.Response.Write("{\"msg\":\"" + ZGZY.Common.JsonHelper.StringFilter(ex.Message) + "\",\"success\":false}");
                userOperateLog.OperateInfo = "菜单功能异常";
                userOperateLog.IfSuccess = false;
                userOperateLog.Description = ZGZY.Common.JsonHelper.StringFilter(ex.Message);
                ZGZY.BLL.UserOperateLog.InsertOperateInfo(userOperateLog);
            }
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}