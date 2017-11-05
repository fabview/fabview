using System;

namespace ZGZY.Model
{
    /// <summary>
    /// 导航菜单类
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 导航菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父级节点id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 菜单标识码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkAddress { get; set; }

        /// <summary>
        /// 导航菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 导航菜单排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }

    }
}
