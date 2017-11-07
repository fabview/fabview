using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace ZGZY.SQLServerDAL
{
    /// <summary>
    /// 用户（SQL Server数据库实现）
    /// </summary>
    public class User : ZGZY.IDAL.IUser
    {
        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        public Model.User GetUserByUserId(string userId)
        {
            string sql = "select top 1 * from tbUser where UserId = @UserId";
            ZGZY.Model.User user = null;
            DataTable dt = ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sql, new SqlParameter("@UserId", userId));
            if (dt.Rows.Count > 0)
            {
                user = new ZGZY.Model.User();
                DataRowToModel(user, dt.Rows[0]);
                return user;
            }
            else
                return null;
        }

        /// <summary>
        /// 根据id获取用户
        /// </summary>
        public Model.User GetUserById(string id)
        {
            string sql = "select * from tbUser where Id = @Id";
            ZGZY.Model.User user = null;
            DataTable dt = ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sql, new SqlParameter("@Id", id));
            if (dt.Rows.Count > 0)
            {
                user = new ZGZY.Model.User();
                DataRowToModel(user, dt.Rows[0]);
                return user;
            }
            else
                return null;
        }

        /// <summary>
        /// 首次登陆强制修改密码
        /// </summary>
        public bool InitUserPwd(Model.User user)
        {
            string sql = "update tbUser set UserPwd = @UserPwd,IfChangePwd = @IfChangePwd where Id = @Id";
            SqlParameter[] paras = { 
                                   new SqlParameter("@UserPwd",user.UserPwd),
                                   new SqlParameter("@IfChangePwd",true),
                                   new SqlParameter("@Id",user.Id)
                                   };
            object obj = ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sql, paras);
            if (Convert.ToInt32(obj) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public bool ChangePwd(Model.User user)
        {
            string sql = "update tbUser set UserPwd = @UserPwd where Id = @Id";
            SqlParameter[] paras = { 
                                   new SqlParameter("@UserPwd",user.UserPwd),
                                   new SqlParameter("@Id",user.Id)
                                   };
            object obj = ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sql, paras);
            if (Convert.ToInt32(obj) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public Model.User UserLogin(string loginId, string loginPwd)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("select top 1 Id,UserId,UserName,UserPwd,IsAble,IfChangePwd,AddDate,Description from tbUser ");
            sbSql.Append("where UserId=@UserId and UserPwd=@UserPwd");
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserId",loginId),
                                       new SqlParameter("@UserPwd",loginPwd)
                                       };
            ZGZY.Model.User user = null;
            DataTable dt = ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sbSql.ToString(), paras);
            if (dt.Rows.Count > 0)
            {
                user = new Model.User();
                //DataRowToModel(user, dt.Rows[0]);
                //只取部分属性写入cookie（防止某些列特别长，例如description，导致cookie过长）：
                if (!DBNull.Value.Equals(dt.Rows[0]["Id"]))
                    user.Id = int.Parse(dt.Rows[0]["Id"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["UserId"]))
                    user.UserId = dt.Rows[0]["UserId"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["UserName"]))
                    user.UserName = dt.Rows[0]["UserName"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["UserPwd"]))
                    user.UserPwd = dt.Rows[0]["UserPwd"].ToString();
                if (!DBNull.Value.Equals(dt.Rows[0]["IsAble"]))
                    user.IsAble = bool.Parse(dt.Rows[0]["IsAble"].ToString());
                if (!DBNull.Value.Equals(dt.Rows[0]["IfChangePwd"]))
                    user.IfChangePwd = bool.Parse(dt.Rows[0]["IfChangePwd"].ToString());
                return user;
            }
            return user;
        }

        /// <summary>
        /// 根据用户id判断用户是否可用
        /// </summary>
        public Model.User CheckLoginByUserId(string userId)
        {
            string sql = "select top 1 Id,UserId,UserName,UserPwd,IsAble,IfChangePwd,AddDate,Description from tbUser where UserId=@UserId";
            ZGZY.Model.User user = null;
            DataTable dt = ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sql, new SqlParameter("@UserId", userId));
            if (dt.Rows.Count > 0)
            {
                user = new ZGZY.Model.User();
                DataRowToModel(user, dt.Rows[0]);
                return user;
            }
            else
                return null;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        public int AddUser(Model.User user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into tbUser(UserId,UserName,UserPwd,IsAble,IfChangePwd,Description)");
            strSql.Append(" values ");
            strSql.Append("(@UserId,@UserName,@UserPwd,@IsAble,@IfChangePwd,@Description)");
            strSql.Append(";SELECT @@IDENTITY");   //返回插入用户的主键
            SqlParameter[] paras = { 
                                   new SqlParameter("@UserId",user.UserId),
                                   new SqlParameter("@UserName",user.UserName),
                                   new SqlParameter("@UserPwd",user.UserPwd),
                                   new SqlParameter("@IsAble",user.IsAble),
                                   new SqlParameter("@IfChangePwd",user.IfChangePwd),
                                   new SqlParameter("@Description",user.Description)
                                   };
            return Convert.ToInt32(ZGZY.Common.SqlHelper.ExecuteScalar(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), paras));
        }

        /// <summary>
        /// 删除用户（可批量删除，删除用户同时删除对应的权限和所处的部门）
        /// </summary>
        public bool DeleteUser(string idList)
        {
            List<string> list = new List<string>();
            list.Add("delete from tbUser where Id in (" + idList + ")");
            list.Add("delete from tbUserRole where UserId in (" + idList + ")");
            list.Add("delete from tbUserDepartment where UserId in (" + idList + ")");

            try
            {
                ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, list);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        public bool EditUser(Model.User user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update tbUser set ");
            strSql.Append("UserId=@UserId,UserName=@UserName,IsAble=@IsAble,IfChangePwd=@IfChangePwd,Description=@Description");
            strSql.Append(" where Id=@Id");

            SqlParameter[] paras = { 
                                   new SqlParameter("@UserId",user.UserId),
                                   new SqlParameter("@UserName",user.UserName),
                                   new SqlParameter("@IsAble",user.IsAble),
                                   new SqlParameter("@IfChangePwd",user.IfChangePwd),
                                   new SqlParameter("@Description",user.Description),
                                   new SqlParameter("@Id",user.Id)
                                   };
            object obj = ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), paras);
            if (Convert.ToInt32(obj) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取用户信息（“我的信息”）
        /// </summary>
        public DataTable GetUserInfo(int userId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select u.UserId,u.UserName,u.AddDate,r.RoleName,d.DepartmentName from tbUser u");
            strSql.Append(" left join tbUserRole ur on u.Id = ur.UserId");
            strSql.Append(" left join tbRole r on ur.RoleId = r.Id");
            strSql.Append(" left join tbUserDepartment ud on u.Id= ud.UserId");
            strSql.Append(" left join tbDepartment d on ud.DepartmentId = d.Id");
            strSql.Append(" where u.Id = @userId");
            return ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), new SqlParameter("@userId", userId));
        }

        /// <summary>
        /// 把DataRow行转成实体类对象
        /// </summary>
        private void DataRowToModel(ZGZY.Model.User model, DataRow dr)
        {
            if (!DBNull.Value.Equals(dr["Id"]))
                model.Id = int.Parse(dr["Id"].ToString());
            if (!DBNull.Value.Equals(dr["UserId"]))
                model.UserId = dr["UserId"].ToString();
            if (!DBNull.Value.Equals(dr["UserName"]))
                model.UserName = dr["UserName"].ToString();
            if (!DBNull.Value.Equals(dr["UserPwd"]))
                model.UserPwd = dr["UserPwd"].ToString();
            if (!DBNull.Value.Equals(dr["IsAble"]))
                model.IsAble = bool.Parse(dr["IsAble"].ToString());
            if (!DBNull.Value.Equals(dr["IfChangePwd"]))
                model.IfChangePwd = bool.Parse(dr["IfChangePwd"].ToString());
            if (!DBNull.Value.Equals(dr["AddDate"]))
                model.AddDate = Convert.ToDateTime(dr["AddDate"]);
            if (!DBNull.Value.Equals(dr["Description"]))
                model.Description = dr["Description"].ToString();
        }

    }
}
