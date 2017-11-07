using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace ZGZY.SQLServerDAL
{
    /// <summary>
    /// 部门（SQL Server数据库实现）
    /// </summary>
    public class Department : ZGZY.IDAL.IDepartment
    {
        /// <summary>
        /// 根据用户id获取用户部门
        /// </summary>
        public DataTable GetDepartmentByUserId(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select d.Id departmentid,d.DepartmentName departmentname from tbUserDepartment ud");
            sb.Append(" join tbDepartment d on d.Id=ud.DepartmentId");
            sb.Append(" where ud.UserId=@Id");

            return ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sb.ToString(), new SqlParameter("@Id", id));
        }

        /// <summary>
        /// 根据条件获取部门
        /// </summary>
        public DataTable GetAllDepartment(string where)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from tbDepartment");
            if (!string.IsNullOrEmpty(where))
            {
                strSql.Append(" where " + where);
            }
            strSql.Append(" order by ParentId,Sort");
            return ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        public int AddDepartment(Model.Department department)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into tbDepartment(DepartmentName,ParentId,Sort)");
            strSql.Append(" values ");
            strSql.Append("(@DepartmentName,@ParentId,@Sort)");
            strSql.Append(";SELECT @@IDENTITY");   //返回插入用户的主键
            SqlParameter[] paras = { 
                                   new SqlParameter("@DepartmentName",department.DepartmentName),
                                   new SqlParameter("@ParentId",department.ParentId),
                                   new SqlParameter("@Sort",department.Sort)
                                   };
            return Convert.ToInt32(ZGZY.Common.SqlHelper.ExecuteScalar(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), paras));
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        public bool EditDepartment(Model.Department department)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update tbDepartment set ");
            strSql.Append("DepartmentName=@DepartmentName,Sort=@Sort ");
            strSql.Append("where Id=@Id");

            SqlParameter[] paras = { 
                                   new SqlParameter("@DepartmentName",department.DepartmentName),
                                   new SqlParameter("@Sort",department.Sort),
                                   new SqlParameter("@Id",department.Id)
                                   };
            object obj = ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), paras);
            if (Convert.ToInt32(obj) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        public bool DeleteDepartment(string departmentIds)
        {
            List<string> list = new List<string>();
            list.Add("delete from tbDepartment where Id in (" + departmentIds + ")");
            list.Add("delete from tbUserDepartment where DepartmentId in (" + departmentIds + ")");

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
        /// 获取部门下的用户个数
        /// </summary>
        public int GetDepartmentUserCount(string departmentIds)
        {
            string sql = "select COUNT(*) from tbUserDepartment ud join tbUser u on ud.UserId = u.Id where ud.DepartmentId in (" + departmentIds + ")";
            object count = ZGZY.Common.SqlHelper.ExecuteScalar(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sql, null);
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// 获取部门下的用户（分页）
        /// </summary>
        public DataTable GetPagerDepartmentUser(string departmentIds, string order, int pageSize, int pageIndex)
        {
            int beginIndex = (pageIndex - 1) * pageSize + 1;   //分页开始页码
            int endIndex = pageIndex * pageSize;   //分页结束页码
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select distinct(T.Id),T.UserId,T.UserName,T.IsAble,T.IfChangePwd,T.AddDate from (");
            strSql.Append(" select row_number() over(order by u." + order + ")");
            strSql.Append(" as Rownum,u.Id,u.UserId,u.UserName,u.IsAble,u.IfChangePwd,u.AddDate from tbDepartment d");
            strSql.Append(" join tbUserDepartment ud on d.Id = ud.DepartmentId");
            strSql.Append(" join tbUser u on ud.UserId = u.Id");
            strSql.Append(" where d.Id in (" + departmentIds + ")) as T");
            strSql.Append(" where T.Rownum between " + beginIndex + " and " + endIndex + "");
            return ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
        }

    }
}
