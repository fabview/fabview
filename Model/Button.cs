using System;

namespace ZGZY.Model
{
    /// <summary>
    /// 按钮类
    /// </summary>
    public class Button
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 按钮标识码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 按钮图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 按钮排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }

        /// <summary>
        /// 按钮简介
        /// </summary>
        public string Description { get; set; }

    }
}
