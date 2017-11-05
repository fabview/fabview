using System;

namespace ZGZY.Model
{
    /// <summary>
    /// 用户操作记录类
    /// </summary>
    public class UserOperateLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 操作的用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 操作用户的ip
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OperateInfo { get; set; }

        /// <summary>
        /// 详细描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否操作成功
        /// </summary>
        public bool IfSuccess { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateDate { get; set; }

    }
}
