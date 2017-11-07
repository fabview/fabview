using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace ZGZY.SQLServerDAL
{
    /// <summary>
    /// 用户角色（SQL Server数据库实现）
    /// </summary>
    public class UserRole : ZGZY.IDAL.IUserRole
    {
        /// <summary>
        /// 设置用户角色（单个用户）
        /// </summary>
        /// <param name="role_addList">要增加的</param>
        /// <param name="role_deleteList">要删除的</param>
        public bool SetRoleSingle(List<Model.UserRole> role_addList, List<Model.UserRole> role_deleteList)
        {
            Hashtable sqlStringList = new Hashtable();
            for (int i = 0; i < role_deleteList.Count; i++)  //删除的用户角色
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("delete from tbUserRole ");
                sb.Append("where UserId=@UserId and RoleId=@RoleId");
                SqlParameter[] para1 = {
					                   new SqlParameter("@UserId", SqlDbType.Int,10),
                                       new SqlParameter("@RoleId", SqlDbType.Int,10)
                                       };
                para1[0].Value = role_deleteList[i].UserId;
                para1[1].Value = role_deleteList[i].RoleId;
                sqlStringList.Add(sb, para1);
            }
            for (int i = 0; i < role_addList.Count; i++)  //新增的用户角色
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into tbUserRole(");
                sb.Append("UserId,RoleId)");
                sb.Append(" values (");
                sb.Append("@UserId,@RoleId)");
                sb.Append(";select @@IDENTITY");
                SqlParameter[] para2 = { 
                                       new SqlParameter("@UserId", SqlDbType.Int,10),
                                       new SqlParameter("@RoleId", SqlDbType.Int,10)
                                       };
                para2[0].Value = role_addList[i].UserId;
                para2[1].Value = role_addList[i].RoleId;
                sqlStringList.Add(sb, para2);    //【【sb不能ToString() 否则报hashtable不能有相同键的错误】】
            }
            try
            {
                ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, sqlStringList);   //批量插入（事务）
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 设置用户角色（批量设置）
        /// </summary>
        /// <param name="role_addList">要增加的</param>
        /// <param name="role_deleteList">要删除的</param>
        public bool SetRoleBatch(List<Model.UserRole> role_addList, List<Model.UserRole> role_deleteList)
        {
            Hashtable sqlStringListDelete = new Hashtable();
            Hashtable sqlStringListAdd = new Hashtable();
            for (int i = 0; i < role_deleteList.Count; i++)  //删除的用户角色
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("delete from tbUserRole ");
                sb.Append("where UserId=@UserId");
                SqlParameter[] para1 = {
                                       new SqlParameter("@UserId", SqlDbType.Int,10)   //批量设置先删除当前用户的所有角色
                                       };
                para1[0].Value = role_deleteList[i].UserId;
                sqlStringListDelete.Add(sb, para1);
            }
            for (int i = 0; i < role_addList.Count; i++)  //新增的用户角色
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into tbUserRole(");
                sb.Append("UserId,RoleId)");
                sb.Append(" values (");
                sb.Append("@UserId,@RoleId)");
                sb.Append(";select @@IDENTITY");
                SqlParameter[] para2 = { 
                                       new SqlParameter("@UserId", SqlDbType.Int,10),
                                       new SqlParameter("@RoleId", SqlDbType.Int,10)
                                       };
                para2[0].Value = role_addList[i].UserId;
                para2[1].Value = role_addList[i].RoleId;
                sqlStringListAdd.Add(sb, para2);    //【【sb不能ToString() 否则报hashtable不能有相同键的错误】】
            }
            try
            {
                ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, sqlStringListDelete);   //先删
                ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, sqlStringListAdd);   //再加
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
