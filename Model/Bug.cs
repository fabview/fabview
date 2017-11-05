using System;

namespace ZGZY.Model
{
    /// <summary>
    /// 用户反馈类
    /// </summary>
    public class Bug
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 提交bug的用户
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 提交bug用户的ip
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        /// bug具体内容
        /// </summary>
        public string BugInfo { get; set; }

        /// <summary>
        /// 站长回复
        /// </summary>
        public string BugReply { get; set; }

        /// <summary>
        /// 提交bug的时间
        /// </summary>
        public DateTime BugDate { get; set; }

        /// <summary>
        /// 是否显示（防止广告，人工审核后才显示）
        /// </summary>
        public bool IfShow { get; set; }

        /// <summary>
        /// bug是否已解决
        /// </summary>
        public bool IfSolve { get; set; }

    }
}
