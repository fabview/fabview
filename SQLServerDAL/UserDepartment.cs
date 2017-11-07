using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace ZGZY.SQLServerDAL
{
    /// <summary>
    /// 用户部门（SQL Server数据库实现）
    /// </summary>
    public class UserDepartment : ZGZY.IDAL.IUserDepartment
    {
        /// <summary>
        /// 设置用户部门（单个用户）
        /// </summary>
        /// <param name="dep_addList">要增加的</param>
        /// <param name="dep_deleteList">要删除的</param>
        public bool SetDepartmentSingle(List<Model.UserDepartment> dep_addList, List<Model.UserDepartment> dep_deleteList)
        {
            Hashtable sqlStringList = new Hashtable();
            for (int i = 0; i < dep_deleteList.Count; i++)  //删除的用户角色
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("delete from tbUserDepartment ");
                sb.Append("where UserId=@UserId and DepartmentId=@DepartmentId");
                SqlParameter[] para1 = {
					                   new SqlParameter("@UserId", SqlDbType.Int,10),
                                       new SqlParameter("@DepartmentId", SqlDbType.Int,10)
                                       };
                para1[0].Value = dep_deleteList[i].UserId;
                para1[1].Value = dep_deleteList[i].DepartmentId;
                sqlStringList.Add(sb, para1);
            }
            for (int i = 0; i < dep_addList.Count; i++)  //新增的用户角色
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into tbUserDepartment(");
                sb.Append("UserId,DepartmentId)");
                sb.Append(" values (");
                sb.Append("@UserId,@DepartmentId)");
                sb.Append(";select @@IDENTITY");
                SqlParameter[] para2 = { 
                                       new SqlParameter("@UserId", SqlDbType.Int,10),
                                       new SqlParameter("@DepartmentId", SqlDbType.Int,10)
                                       };
                para2[0].Value = dep_addList[i].UserId;
                para2[1].Value = dep_addList[i].DepartmentId;
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
        /// 设置用户部门（批量设置）
        /// </summary>
        /// <param name="dep_addList">要增加的</param>
        /// <param name="dep_deleteList">要删除的</param>
        public bool SetDepartmentBatch(List<Model.UserDepartment> dep_addList, List<Model.UserDepartment> dep_deleteList)
        {
            Hashtable sqlStringListDelete = new Hashtable();
            Hashtable sqlStringListAdd = new Hashtable();
            for (int i = 0; i < dep_deleteList.Count; i++)  //删除的用户角色
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("delete from tbUserDepartment ");
                sb.Append("where UserId=@UserId");
                SqlParameter[] para1 = {
                                       new SqlParameter("@UserId", SqlDbType.Int,10)   //批量设置先删除当前用户的所有角色
                                       };
                para1[0].Value = dep_deleteList[i].UserId;
                sqlStringListDelete.Add(sb, para1);
            }
            for (int i = 0; i < dep_addList.Count; i++)  //新增的用户角色
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into tbUserDepartment(");
                sb.Append("UserId,DepartmentId)");
                sb.Append(" values (");
                sb.Append("@UserId,@DepartmentId)");
                sb.Append(";select @@IDENTITY");
                SqlParameter[] para2 = { 
                                       new SqlParameter("@UserId", SqlDbType.Int,10),
                                       new SqlParameter("@DepartmentId", SqlDbType.Int,10)
                                       };
                para2[0].Value = dep_addList[i].UserId;
                para2[1].Value = dep_addList[i].DepartmentId;
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
