using System;

namespace ZGZY.Model
{
    /// <summary>
    /// 用户类
    /// </summary>
    public class User
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户登录id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户登录密码
        /// </summary>
        public string UserPwd { get; set; }

        /// <summary>
        /// 用户是否被启用
        /// </summary>
        public bool IsAble { get; set; }

        /// <summary>
        /// 用户是否修改密码（强制第一次登陆修改密码）
        /// </summary>
        public bool IfChangePwd { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }

        /// <summary>
        /// 用户简介
        /// </summary>
        public string Description { get; set; }

    }
}
