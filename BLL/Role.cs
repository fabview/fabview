using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ZGZY.BLL
{
    /// <summary>
    /// 角色（BLL）
    /// </summary>
    public class Role
    {
        private static readonly ZGZY.IDAL.IRole dal = ZGZY.DALFactory.Factory.GetRoleDAL();

        /// <summary>
        /// 根据用户id获取用户角色
        /// </summary>
        public DataTable GetRoleByUserId(int id)
        {
            return dal.GetRoleByUserId(id);
        }

        /// <summary>
        /// 根据条件获取角色
        /// </summary>
        public string GetAllRole(string where)
        {
            return ZGZY.Common.JsonHelper.ToJson(dal.GetAllRole(where));
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

        /// <summary>
        /// 添加角色
        /// </summary>
        public int AddRole(Model.Role role)
        {
            ZGZY.Model.Role roleCompare = dal.GetRoleByRoleName(role.RoleName);
            if (roleCompare != null)
            {
                throw new Exception("已经存在此角色！");
            }
            return dal.AddRole(role);
        }

        /// <summary>
        /// 删除角色（删除角色同时删除对应的：用户角色/角色菜单按钮【即权限】）
        /// </summary>
        public bool DeleteRole(int id)
        {
            return dal.DeleteRole(id);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        public bool EditRole(Model.Role role, string originalRoleName)
        {
            if (role.RoleName != originalRoleName && dal.GetRoleByRoleName(role.RoleName) != null)
            {
                throw new Exception("已经存在此角色！");
            }
            return dal.EditRole(role);
        }

        /// <summary>
        /// 角色授权
        /// </summary>
        /// <param name="roleId">要授权的角色Id</param>
        /// <param name="roleMenuButtonId">菜单按钮Id 格式：5 1,5 2,7 1,10 1,11 1</param>
        public bool Authorize(int roleId, string roleMenuButtonId)
        {
            DataTable dt_role_menu_button_old = dal.GetMenuButtonIdByRoleId(roleId);  //此角色下修改之前有的菜单和按钮权限
            List<ZGZY.Model.RoleMenuButton> role_menu_button_addlist = new List<Model.RoleMenuButton>();    //需要新添加的（用户新勾选的）
            List<ZGZY.Model.RoleMenuButton> role_menu_button_deletelist = new List<Model.RoleMenuButton>();   //需要删除的（用户去除勾选的）

            string[] role_menu_button_userselect = roleMenuButtonId.Split(','); //用户提交的菜单/按钮Id
            //用户去掉勾选的按钮（要删除本角色的按钮）
            for (int i = 0; i < dt_role_menu_button_old.Rows.Count; i++)
            {
                //拼接出跟role_menu_button_userselect数组元素格式一致，方便比较
                string role_menu_button_old = dt_role_menu_button_old.Rows[i]["MenuId"].ToString() + " " + dt_role_menu_button_old.Rows[i]["ButtonId"].ToString();

                //等于-1说明用户去掉勾选了某个按钮 需要删除
                if (Array.IndexOf(role_menu_button_userselect, role_menu_button_old) == -1)
                {
                    ZGZY.Model.RoleMenuButton roleMenuButton = new Model.RoleMenuButton();
                    roleMenuButton.RoleId = roleId;
                    roleMenuButton.MenuId = Convert.ToInt32(role_menu_button_old.Split(' ')[0]);
                    roleMenuButton.ButtonId = Convert.ToInt32(role_menu_button_old.Split(' ')[1]);
                    role_menu_button_deletelist.Add(roleMenuButton);
                }
            }

            //用户新勾选的按钮（要添加本角色下的按钮）
            if (!string.IsNullOrEmpty(roleMenuButtonId))
            {
                List<int> listParentMenuId = new List<int>();   //需要添加的父目录id
                for (int i = 0; i < role_menu_button_userselect.Length; i++)
                {
                    int menuId = Convert.ToInt32(role_menu_button_userselect[i].Split(' ')[0]);
                    int buttonId = Convert.ToInt32(role_menu_button_userselect[i].Split(' ')[1]);

                    //等于0那么原来的按钮没有 是用户新勾选的
                    if (dt_role_menu_button_old.Select(string.Format("MenuId={0} and ButtonId={1}", menuId, buttonId)).Length == 0)
                    {
                        //新勾选按钮的时候要判断是否有根节点在数据库里，没有得加上根节点，否则登陆不显示树
                        int parentId = dal.GetParentMenuId(roleId, menuId);
                        if (parentId != -1 && !listParentMenuId.Contains(parentId))
                        {
                            listParentMenuId.Add(parentId);
                            ZGZY.Model.RoleMenuButton roleParentMenu = new Model.RoleMenuButton();
                            roleParentMenu.RoleId = roleId;
                            roleParentMenu.MenuId = parentId;
                            roleParentMenu.ButtonId = 0;
                            role_menu_button_addlist.Add(roleParentMenu);
                        }

                        ZGZY.Model.RoleMenuButton roleMenuButton = new Model.RoleMenuButton();
                        roleMenuButton.RoleId = roleId;
                        roleMenuButton.MenuId = menuId;
                        roleMenuButton.ButtonId = buttonId;
                        role_menu_button_addlist.Add(roleMenuButton);
                    }
                }
            }
            if (role_menu_button_addlist.Count != 0 || role_menu_button_deletelist.Count != 0)
                return dal.Authorize(role_menu_button_addlist, role_menu_button_deletelist);
            else
                return true;
        }

        /// <summary>
        /// 获取权限下的用户（分页）
        /// </summary>
        public string GetPagerRoleUser(int roleId, string order, int pageSize, int pageIndex)
        {
            if (ZGZY.Common.SqlInjection.GetString(order))   //简单的sql注入过滤
                order = "AddDate asc";
            int totalCount = dal.GetRoleUserCount(roleId);
            DataTable dt = dal.GetPagerRoleUser(roleId, order, pageSize, pageIndex);

            string strjson = ZGZY.Common.JsonHelper.ToJson(dt);
            return "{\"total\": " + totalCount.ToString() + ",\"rows\":" + strjson + "}";
        }

    }
}
