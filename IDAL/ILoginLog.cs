using System;
using System.Data;
using System.Collections.Generic;

namespace ZGZY.IDAL
{
    /// <summary>
    /// 登陆日志接口（不同的数据库访问类实现接口达到多数据库的支持）
    /// </summary>
    public interface ILoginLog
    {
        /// <summary>
        /// 根据条件查询记录总数
        /// </summary>
        int GetTotalCount(string where);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        DataTable GetPager(string strWhere, string orderBy, string order, int pageIndex, int pageSize);

        /// <summary>
        /// 判断登录：如果30分钟内同一个ip连续5次登录失败，那么30分钟后才可以继续登录
        /// </summary>
        DateTime? CheckLogin(string ip, out DateTime? lastLoginTime);

        /// <summary>
        /// 记录用户登录日志
        /// </summary>
        bool WriteLoginLog(ZGZY.Model.LoginLog loginInfo);

    }
}
