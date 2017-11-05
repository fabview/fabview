using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ZGZY.BLL
{
    /// <summary>
    /// 用户登录信息（BLL）
    /// </summary>
    public class LoginLog
    {
        //BLL不直接访问底层数据库，调用工厂方法
        //private static ZGZY.IDAL.ILoginInfo dal;     //方式一：数据访问对象dal为null，通过构造函数赋值
        //public LoginInfo()
        //{
        //    //在BLL都是通过工厂创建的对象实例
        //    dal = ZGZY.DALFactory.Factory.GetLoginInfoDALs();
        //}

        //方式二，初始化的时候直接赋值（dal为静态的，只初始化一次，所有的实例通通过这一个对象去操作）：
        private static readonly ZGZY.IDAL.ILoginLog dal = ZGZY.DALFactory.Factory.GetLoginInfoDAL();

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="strWhere">分页条件</param>
        /// <param name="orderBy">排序列</param>
        /// <param name="order">排序方式：asc、desc</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        public string GetPager(string strWhere, string orderBy, string order, int pageIndex, int pageSize)
        {
            //先查总数再执行分页方法，每次都连了两次数据库。只有这个类用的此方法分页，其他类都是调用一个带输出参数的存储过程(sp_Pager)实现的分页，一次数据库连接（输出参数就是总记录数）
            int totalCount = dal.GetTotalCount(strWhere);
            DataTable dt = dal.GetPager(strWhere, orderBy, order, pageIndex, pageSize);

            string strjson = ZGZY.Common.JsonHelper.ToJson(dt);
            return "{\"total\": " + totalCount.ToString() + ",\"rows\":" + strjson + "}";
        }

        /// <summary>
        /// 判断登录：如果30分钟内同一个ip连续5次登录失败，那么30分钟后才可以继续登录
        /// </summary>
        /// <param name="ip">用户ip</param>
        /// <param name="lastLoginTime">输出参数：如果30分钟没有5次的失败登录，那么返回null；如果有，就返回下一次可以登录的时间</param>
        public DateTime? CheckLogin(string ip, out DateTime? lastLoginTime)
        {
            return dal.CheckLogin(ip, out lastLoginTime);
        }

        /// <summary>
        /// 记录用户登录日志
        /// </summary>
        public bool WriteLoginLog(Model.LoginLog loginInfo)
        {
            return dal.WriteLoginLog(loginInfo);
        }


    }
}
