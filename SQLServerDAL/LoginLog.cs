using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using log4net;

namespace ZGZY.SQLServerDAL
{
    /// <summary>
    /// 登陆日志（SQL Server数据库实现）
    /// </summary>
    public class LoginLog : ZGZY.IDAL.ILoginLog
    {
        /// <summary>
        /// 根据条件查询记录总数
        /// </summary>
        public int GetTotalCount(string where)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from tbLoginLog");
            if (!string.IsNullOrEmpty(where))
            {
                strSql.Append(" where " + where);
            }
            object count = ZGZY.Common.SqlHelper.ExecuteScalar(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="strWhere">分页条件</param>
        /// <param name="orderBy">排序列</param>
        /// <param name="order">排序方式：asc、desc</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        public DataTable GetPager(string strWhere, string orderBy, string order, int pageIndex, int pageSize)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from (");
            strSql.Append("select ROW_NUMBER() over (");
            if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(order))
                strSql.Append("order by T." + orderBy + " " + order);
            else
                strSql.Append("order by T.LoginDate asc");
            strSql.Append(") as Row,T.* from tbLoginLog T");
            if (!string.IsNullOrEmpty(strWhere))
                strSql.Append(" where " + strWhere);
            strSql.Append(") TT");
            strSql.AppendFormat(" Where TT.Row between {0} and {1}", pageSize * (pageIndex - 1) + 1, pageSize * pageIndex);
            return ZGZY.Common.SqlHelper.GetDataTable(ZGZY.Common.SqlHelper.connStr, CommandType.Text, strSql.ToString(), null);
        }

        /// <summary>
        /// 判断登录：如果30分钟内同一个ip连续5次登录失败，那么30分钟后才可以继续登录
        /// </summary>
        /// <param name="ip">用户ip</param>
        /// <param name="lastLoginTime">输出参数：如果30分钟没有5次的失败登录，那么返回null；如果有，就返回下一次可以登录的时间</param>
        public DateTime? CheckLogin(string ip, out DateTime? lastLoginTime)
        {
            SqlParameter[] paras = { 
                                   new SqlParameter("@ip",SqlDbType.VarChar),
                                   new SqlParameter("@lastErrorLoginTime",SqlDbType.DateTime)
                                   };
            paras[0].Value = ip;
            paras[1].Direction = ParameterDirection.Output;  //指定为输出参数
            ZGZY.Common.SqlHelper.ExecuteScalar(ZGZY.Common.SqlHelper.connStr, CommandType.StoredProcedure, "sp_CheckLogin", paras);

            if (paras[1].Value != DBNull.Value)
                lastLoginTime = Convert.ToDateTime(paras[1].Value);
            else
                lastLoginTime = null;

            return lastLoginTime;
        }

        /// <summary>
        /// 记录用户登录日志
        /// </summary>
        public bool WriteLoginLog(Model.LoginLog loginInfo)
        {
            string sql = "insert into tbLoginLog(UserName,UserIp,City,Success) values (@UserName,@UserIp,@City,@Success)";
            SqlParameter[] paras = { 
                                   new SqlParameter("UserName",loginInfo.UserName),
                                   new SqlParameter("UserIp",loginInfo.UserIp),
                                   new SqlParameter("City",loginInfo.City),
                                   new SqlParameter("Success",loginInfo.Success)                              
                                   };
            object count = ZGZY.Common.SqlHelper.ExecuteNonQuery(ZGZY.Common.SqlHelper.connStr, CommandType.Text, sql, paras);
            if (Convert.ToInt32(count) > 0)
                return true;
            else
            {
                //登录日志记录不成功log4net输出
                //ILog log = log4net.LogManager.GetLogger(typeof(LoginLog));  //得到日志器
                //log.WarnFormat("登录日志记录失败！参数：{0},{1},{2}", loginInfo.UserName, loginInfo.UserIp, loginInfo.City);    //记录日志
                return false;
            }
        }

    }
}
