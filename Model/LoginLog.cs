using System;

namespace ZGZY.Model
{
    /// <summary>
    /// 用户登录日志类
    /// </summary>
    public class LoginLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户登录ip
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        /// 用户登录城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginDate { get; set; }

    }
}
