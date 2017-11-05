using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ZGZY.BLL
{
    /// <summary>
    /// 用户操作记录（BLL）
    /// </summary>
    public class UserOperateLog
    {
        private static readonly ZGZY.IDAL.IUserOperateLog dal = ZGZY.DALFactory.Factory.GetUserOperateLogDAL();

        /// <summary>
        /// 写入操作日志
        /// </summary>
        public static void InsertOperateInfo(Model.UserOperateLog userOperateLog)
        {
            dal.InsertOperateLog(userOperateLog);
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

    }
}
