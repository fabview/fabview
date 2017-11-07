using System.Collections.Generic;

namespace ZGZY.IDAL
{
    /// <summary>
    /// 用户角色接口（不同的数据库访问类实现接口达到多数据库的支持）
    /// </summary>
    public interface IUserRole
    {
        /// <summary>
        /// 设置用户角色（单个用户）
        /// </summary>
        /// <param name="role_addList">要增加的</param>
        /// <param name="role_deleteList">要删除的</param>
        bool SetRoleSingle(List<ZGZY.Model.UserRole> role_addList, List<ZGZY.Model.UserRole> role_deleteList);

        /// <summary>
        /// 设置用户角色（批量设置）
        /// </summary>
        /// <param name="role_addList">要增加的</param>
        /// <param name="role_deleteList">要删除的</param>
        bool SetRoleBatch(List<ZGZY.Model.UserRole> role_addList, List<ZGZY.Model.UserRole> role_deleteList);

    }
}
