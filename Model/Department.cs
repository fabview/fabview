using System;

namespace ZGZY.Model
{
    /// <summary>
    /// 部门类
    /// </summary>
    public class Department
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 父节点id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 菜单排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }

    }
}
