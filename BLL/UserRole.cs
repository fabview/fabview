using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ZGZY.BLL
{
    /// <summary>
    /// 用户角色（BLL）
    /// </summary>
    public class UserRole
    {
        private static readonly ZGZY.IDAL.IUserRole dal = ZGZY.DALFactory.Factory.GetUserRoleDAL();

        /// <summary>
        /// 设置用户角色（单个用户）
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="roleIds">角色id，多个逗号隔开</param>
        public bool SetRoleSingle(int userId, string roleIds)
        {
            DataTable dt_user_role_old = new ZGZY.BLL.Role().GetRoleByUserId(userId);  //用户之前拥有的角色
            List<ZGZY.Model.UserRole> role_addList = new List<ZGZY.Model.UserRole>();     //需要插入角色的sql语句集合
            List<ZGZY.Model.UserRole> role_deleteList = new List<ZGZY.Model.UserRole>();     //需要删除角色的sql语句集合

            string[] str_role = roleIds.Trim(',').Split(',');    //传过来用户勾选的角色（有去勾的也有新勾选的）

            ZGZY.Model.UserRole userroledelete = null;
            ZGZY.Model.UserRole userroleadd = null;
            //用户去掉勾选的角色（要删除本用户的角色）
            for (int i = 0; i < dt_user_role_old.Rows.Count; i++)
            {
                //等于-1说明用户去掉勾选了某个角色 需要删除
                if (Array.IndexOf(str_role, dt_user_role_old.Rows[i]["roleid"].ToString()) == -1)
                {
                    userroledelete = new ZGZY.Model.UserRole();
                    userroledelete.RoleId = Convert.ToInt32(dt_user_role_old.Rows[i]["roleid"].ToString());
                    userroledelete.UserId = userId;
                    role_deleteList.Add(userroledelete);
                }
            }

            //用户新勾选的角色（要添加本用户的角色）
            if (!string.IsNullOrEmpty(roleIds))
            {
                for (int j = 0; j < str_role.Length; j++)
                {
                    //等于0那么原来的角色没有 是用户新勾选的
                    if (dt_user_role_old.Select("roleid = '" + str_role[j] + "'").Length == 0)
                    {
                        userroleadd = new ZGZY.Model.UserRole();
                        userroleadd.UserId = userId;
                        userroleadd.RoleId = Convert.ToInt32(str_role[j]);
                        role_addList.Add(userroleadd);
                    }
                }
            }
            if (role_addList.Count == 0 && role_deleteList.Count == 0)
                return true;
            else
                return dal.SetRoleSingle(role_addList, role_deleteList);
        }

        /// <summary>
        /// 设置用户角色（批量设置）
        /// </summary>
        /// <param name="userIds">用户主键，多个逗号隔开</param>
        /// <param name="roleIds">角色id，多个逗号隔开</param>
        public bool SetRoleBatch(string userIds, string roleIds)
        {
            List<ZGZY.Model.UserRole> role_addList = new List<ZGZY.Model.UserRole>();     //需要插入角色的sql语句集合
            List<ZGZY.Model.UserRole> role_deleteList = new List<ZGZY.Model.UserRole>();     //需要删除角色的sql语句集合
            string[] str_userid = userIds.Trim(',').Split(',');
            string[] str_role = roleIds.Trim(',').Split(',');

            ZGZY.Model.UserRole userroledelete = null;
            ZGZY.Model.UserRole userroleadd = null;
            for (int i = 0; i < str_userid.Length; i++)
            {
                //批量设置先删除当前用户的所有角色
                userroledelete = new ZGZY.Model.UserRole();
                userroledelete.UserId = Convert.ToInt32(str_userid[i]);
                role_deleteList.Add(userroledelete);

                if (!string.IsNullOrEmpty(roleIds))
                {
                    //再添加设置的角色
                    for (int j = 0; j < str_role.Length; j++)
                    {
                        userroleadd = new ZGZY.Model.UserRole();
                        userroleadd.UserId = Convert.ToInt32(str_userid[i]);
                        userroleadd.RoleId = Convert.ToInt32(str_role[j]);
                        role_addList.Add(userroleadd);
                    }
                }
            }
            return dal.SetRoleBatch(role_addList, role_deleteList);
        }

    }
}
