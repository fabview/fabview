using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ZGZY.BLL
{
    /// <summary>
    /// 菜单（BLL）
    /// </summary>
    public class Menu
    {
        private static readonly ZGZY.IDAL.IMenu dal = ZGZY.DALFactory.Factory.GetMenuDAL();

        /// <summary>
        /// 根据用户主键id查询用户可以访问的菜单
        /// </summary>
        public string GetUserMenu(int id)
        {
            /**************************************************
             tbMenu.id: 主目录ID
             tbMenu.menuid: menuid等于id的页面，为该id的子节点
             **************************************************/
            DataTable dt = dal.GetUserMenu(id);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            // menuparentid -> tbMenu.parentid
            DataRow[] rows = dt.Select("menuparentid = 0");   //赋权限每个角色都必须有父节点的权限，否则一个都不输出了
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    // menuid -> menuparentid.id
                    sb.Append("{\"id\":\"" + rows[i]["menuid"].ToString() + "\",\"text\":\"" + rows[i]["menuname"].ToString() + "\",\"iconCls\":\"" + rows[i]["icon"].ToString() + "\",\"children\":[");
                    DataRow[] r_list = dt.Select(string.Format("menuparentid={0}", rows[i]["menuid"]));// menuid -> menuparentid.id
                    if (r_list.Length > 0)  //根节点下有子节点
                    {
                        for (int j = 0; j < r_list.Length; j++)
                        {
                            sb.Append("{\"id\":\"" + r_list[j]["menuid"].ToString() + "\",\"text\":\"" + r_list[j]["menuname"].ToString() + "\",\"iconCls\":\"" + r_list[j]["icon"].ToString() + "\",\"attributes\":{\"url\":\"" + r_list[j]["linkaddress"].ToString() + "\"}},");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("]},");
                    }
                    else  //根节点下没有子节点
                    {
                        sb.Append("]},");  //跟上面if条件之外的字符串拼上
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }
            else
            {
                sb.Append("]");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 根据角色id获取此角色可以访问的菜单和菜单下的按钮（编辑角色-菜单使用）
        /// </summary>
        public string GetAllMenu(int roleId)
        {
            DataTable dt = dal.GetAllMenu(roleId);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataRow[] rows = dt.Select("parentid = 0");
            if (rows.Length > 0)
            {
                DataView dataView = new DataView(dt);
                DataTable dtDistinct = dataView.ToTable(true, new string[] { "menuname", "menuid", "parentid" });   //distinct取不重复的子节点
                for (int i = 0; i < rows.Length; i++)
                {
                    sb.Append("{\"id\":\"" + rows[i]["menuid"].ToString() + "\",\"text\":\"" + rows[i]["menuname"].ToString() + "\",\"children\":[");
                    DataRow[] r_list = dtDistinct.Select(string.Format("parentid = {0}", rows[i]["menuid"]));  //取子节点
                    if (r_list.Length > 0)    //根节点下有子节点
                    {
                        for (int j = 0; j < r_list.Length; j++)
                        {
                            sb.Append("{\"id\":\"" + r_list[j]["menuid"].ToString() + "\",\"text\":\"" + r_list[j]["menuname"].ToString() + "\",\"children\":[");
                            DataRow[] r_listButton = dt.Select(string.Format("menuid = {0}", r_list[j]["menuid"]));  //子子节点
                            if (r_listButton.Length > 0)    //有子子节点就遍历进去
                            {
                                for (int k = 0; k < r_listButton.Length; k++)
                                {
                                    //这里不能取表里的roleid，表里的roleid有些是null，后面的赋权限无法进行
                                    //sb.Append("{\"id\":\"" + r_listButton[k]["roleid"].ToString() + "\",\"text\":\"" + r_listButton[k]["buttonname"].ToString() + "\",\"checked\":" + r_listButton[k]["checked"].ToString() + ",\"attributes\":{\"rolemenubuttonid\":\"" + r_listButton[k]["rolemenubuttonid"].ToString() + "\",\"menuid\":\"" + r_listButton[k]["menuid"].ToString() + "\",\"buttonid\":\"" + r_listButton[k]["buttonid"].ToString() + "\"}},");
                                    sb.Append("{\"id\":\"" + roleId + "\",\"text\":\"" + r_listButton[k]["buttonname"].ToString() + "\",\"checked\":" + r_listButton[k]["checked"].ToString() + ",\"attributes\":{\"menuid\":\"" + r_listButton[k]["menuid"].ToString() + "\",\"buttonid\":\"" + r_listButton[k]["buttonid"].ToString() + "\"}},");
                                }
                                sb.Remove(sb.Length - 1, 1);
                                sb.Append("]},");
                            }
                            else
                            {
                                sb.Append("]},");    //跟上面if条件之外的字符串拼上
                            }
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("]},");
                    }
                    else    //根节点下没有子节点
                    {
                        sb.Append("]},");    //跟上面if条件之外的字符串拼上
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }
            else
            {
                sb.Append("]");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 根据用户主键id查询用户拥有的权限（后台首页“我的权限”）
        /// </summary>
        public string GetMyAuthority(int id)
        {
            DataTable dt = dal.GetMyAuthority(id);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataRow[] rows = dt.Select("parentid = 0");
            if (rows.Length > 0)
            {
                //distinct取不重复的子节点
                DataView dataView = new DataView(dt);
                DataTable dtDistinct = dataView.ToTable(true, new string[] { "menuname", "menuid", "parentid" });
                for (int i = 0; i < rows.Length; i++)
                {
                    sb.Append("{\"id\":\"" + rows[i]["menuid"].ToString() + "\",\"text\":\"" + rows[i]["menuname"].ToString() + "\",\"children\":[");
                    DataRow[] r_list = dtDistinct.Select(string.Format("parentid = {0}", rows[i]["menuid"]));  //取子节点

                    if (r_list.Length > 0)    //根节点下有子节点
                    {
                        for (int j = 0; j < r_list.Length; j++)
                        {
                            sb.Append("{\"id\":\"" + r_list[j]["menuid"].ToString() + "\",\"text\":\"" + r_list[j]["menuname"].ToString() + "\",\"children\":[");
                            //防止多个角色有同个按钮的权限，distinct一下
                            DataView dataViewBtn = new DataView(dt);
                            DataTable dtDistinctBtn = dataViewBtn.ToTable(true, new string[] { "menuid", "parentid", "buttonid", "buttonname", "checked" });
                            DataRow[] r_listButton = dtDistinctBtn.Select(string.Format("menuid = {0}", r_list[j]["menuid"]));  //子子节点
                            if (r_listButton.Length > 0)    //有子子节点就遍历进去
                            {
                                for (int k = 0; k < r_listButton.Length; k++)
                                {
                                    sb.Append("{\"text\":\"" + r_listButton[k]["buttonname"].ToString() + "\",\"checked\":" + r_listButton[k]["checked"].ToString() + "},");
                                }
                                sb.Remove(sb.Length - 1, 1);
                                sb.Append("]},");
                            }
                            else
                            {
                                sb.Append("]},");    //跟上面if条件之外的字符串拼上
                            }
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("]},");
                    }
                    else     //根节点下没有子节点
                    {
                        sb.Append("]},");   //跟上面if条件之外的字符串拼上
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
            }
            else
            {
                sb.Append("]");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">要取的列名（逗号分开）</param>
        /// <param name="order">排序</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="where">查询条件</param>
        /// <param name="totalCount">总记录数</param>
        public string GetPager(string tableName, string columns, string order, int pageSize, int pageIndex, string where, out int totalCount)
        {
            DataTable dt = ZGZY.Common.SqlPagerHelper.GetPager(tableName, columns, order, pageSize, pageIndex, where, out totalCount);
            return ZGZY.Common.JsonHelper.ToJson(dt);
        }

    }
}
