using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ZGZY.SQLServerDAL
{
    /// <summary>
    /// 权限（SQL Server数据库实现）
    /// </summary>
    public class Authority : ZGZY.IDAL.IAuthority
    {
        /// <summary>
        /// 判断当前用户是否有权限
        /// </summary>
        /// <param name="menuCode">菜单标识码</param>
        /// <param name="buttonCode">按钮标识码</param>
        /// <param name="userId">用户主键</param>
        public bool IfAuthority(string menuCode, string buttonCode, int userId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select count(*) from tbUser u");
            sb.Append(" join tbUserRole ur on u.Id=ur.UserId");
            sb.Append(" join tbRoleMenuButton rmb on ur.RoleId=rmb.RoleId");
            sb.Append(" join tbMenu m on rmb.MenuId=m.Id");
            sb.Append(" join tbButton b on rmb.ButtonId=b.Id");
            sb.Append(" where u.Id=@Id and m.Code=@MenuCode and b.Code=@ButtonCode");
            SqlParameter[] paras = { 
                                   new SqlParameter("@Id",userId),
                                   new SqlParameter("@MenuCode",menuCode),
                                   new SqlParameter("@ButtonCode",buttonCode)
                                   };
            object result = ZGZY.Common.SqlHelper.ExecuteScalar(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sb.ToString(), paras);
            if (Convert.ToInt32(result) > 0)     //如果一个用户有多个角色，角色里的按钮权限有重复，那么这个result可能大于1
                return true;
            else
                return false;
        }

    }
}
