using System;
using System.Data;
using System.Collections.Generic;

namespace ZGZY.IDAL
{
    /// <summary>
    /// 角色接口（不同的数据库访问类实现接口达到多数据库的支持）
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// 根据用户id获取用户角色
        /// </summary>
        DataTable GetRoleByUserId(int id);

        /// <summary>
        /// 根据条件获取角色
        /// </summary>
        DataTable GetAllRole(string where);

        /// <summary>
        /// 根据角色名获取角色
        /// </summary>
        ZGZY.Model.Role GetRoleByRoleName(string roleName);

        /// <summary>
        /// 添加角色
        /// </summary>
        int AddRole(ZGZY.Model.Role role);

        /// <summary>
        /// 删除角色（删除角色同时删除对应的：用户角色/角色菜单按钮【即权限】）
        /// </summary>
        bool DeleteRole(int id);

        /// <summary>
        /// 修改角色
        /// </summary>
        bool EditRole(ZGZY.Model.Role role);

        /// <summary>
        /// 根据角色Id获取对应的菜单按钮权限
        /// </summary>
        DataTable GetMenuButtonIdByRoleId(int roleId);

        /// <summary>
        /// 角色授权
        /// </summary>
        bool Authorize(List<ZGZY.Model.RoleMenuButton> role_menu_button_addlist, List<ZGZY.Model.RoleMenuButton> role_menu_button_deletelist);

        /// <summary>
        /// 获取父节点Id
        /// </summary>
        int GetParentMenuId(int roleId, int menuId);

        /// <summary>
        /// 获取权限下的用户个数
        /// </summary>
        int GetRoleUserCount(int roleId);

        /// <summary>
        /// 获取权限下的用户（分页）
        /// </summary>
        DataTable GetPagerRoleUser(int roleId, string order, int pageSize, int pageIndex);

    }
}
