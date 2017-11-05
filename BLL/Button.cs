using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ZGZY.BLL
{
    /// <summary>
    /// 按钮（BLL）
    /// </summary>
    public class Button
    {
        private static readonly ZGZY.IDAL.IButton dal = ZGZY.DALFactory.Factory.GetButtonDAL();

        /// <summary>
        /// 根据菜单标识码和用户id获取此用户拥有该菜单下的哪些按钮权限
        /// </summary>
        public DataTable GetButtonByMenuCodeAndUserId(string menuCode, int userId)
        {
            return dal.GetButtonByMenuCodeAndUserId(menuCode, userId);
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
