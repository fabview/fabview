using System;
using System.Data;
using System.Collections.Generic;

namespace ZGZY.IDAL
{
    /// <summary>
    /// 部门接口（不同的数据库访问类实现接口达到多数据库的支持）
    /// </summary>
    public interface IDepartment
    {
        /// <summary>
        /// 根据用户id获取用户部门
        /// </summary>
        DataTable GetDepartmentByUserId(int id);

        /// <summary>
        /// 根据条件获取部门
        /// </summary>
        DataTable GetAllDepartment(string where);

        /// <summary>
        /// 添加部门
        /// </summary>
        int AddDepartment(ZGZY.Model.Department department);

        /// <summary>
        /// 修改部门
        /// </summary>
        bool EditDepartment(ZGZY.Model.Department department);

        /// <summary>
        /// 删除部门
        /// </summary>
        bool DeleteDepartment(string departmentIds);

        /// <summary>
        /// 获取部门下的用户个数
        /// </summary>
        int GetDepartmentUserCount(string departmentIds);

        /// <summary>
        /// 获取部门下的用户（分页）
        /// </summary>
        DataTable GetPagerDepartmentUser(string departmentIds, string order, int pageSize, int pageIndex);

    }
}
