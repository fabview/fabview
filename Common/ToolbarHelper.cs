using System;
using System.Data;
using System.Text;

namespace ZGZY.Common
{
    /// <summary>
    /// Easyui Datagrid/Treegrid Toolbar帮助类
    /// </summary>
    public class ToolbarHelper
    {
        /// <summary>
        /// 输出操作按钮
        /// </summary>
        /// <param name="dt">根据用户id和菜单标识码得到的用户可以操作的此菜单下的按钮集合</param>
        /// <param name="pageName">当前页面名称，方便拼接js函数名</param>
        public static string GetToolBar(DataTable dt, string pageName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"toolbar\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                switch (dt.Rows[i]["Code"].ToString())
                {
                    case "add":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_add();\"},");
                        break;
                    case "edit":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_edit();\"},");
                        break;
                    case "delete":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_delete();\"},");
                        break;
                    case "setrole":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_role();\"},");
                        break;
                    case "setdepartment":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_department();\"},");
                        break;
                    case "authorize":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_authorize();\"},");
                        break;
                    case "export":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_export();\"},");
                        break;
                    case "setbutton":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_setbutton();\"},");
                        break;
                    case "expandall":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_expandall();\"},");
                        break;
                    case "collapseall":
                        sb.Append("{\"text\": \"" + dt.Rows[i]["Name"] + "\",\"iconCls\":\"" + dt.Rows[i]["Icon"] + "\",\"handler\":\"" + pageName + "_collapseall();\"},");
                        break;
                    default:
                        //browser不是按钮
                        break;
                }
            }

            bool flag = true;   //是否有浏览权限
            DataRow[] row = dt.Select("code = 'browser'");
            if (row.Length == 0)  //没有浏览权限
            {
                flag = false;
                if (dt.Rows.Count > 0)
                    sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                if (dt.Rows.Count > 1)
                    sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("],\"success\":true,");
            if (flag)
                sb.Append("\"browser\":true}");
            else
                sb.Append("\"browser\":false}");

            return sb.ToString();
        }

    }
}
