using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ZGZY.BLL
{
    /// <summary>
    /// 用户（BLL）
    /// </summary>
    public class User
    {
        private static readonly ZGZY.IDAL.IUser dal = ZGZY.DALFactory.Factory.GetUserDAL();

        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        public Model.User GetUserByUserId(string userId)
        {
            return dal.GetUserByUserId(userId);
        }

        /// <summary>
        /// 根据id获取用户
        /// </summary>
        public Model.User GetUserById(string id)
        {
            return dal.GetUserById(id);
        }

        /// <summary>
        /// 首次登陆强制修改密码
        /// </summary>
        public bool InitUserPwd(Model.User user)
        {
            return dal.InitUserPwd(user);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public bool ChangePwd(Model.User user)
        {
            return dal.ChangePwd(user);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public Model.User UserLogin(string loginId, string loginPwd)
        {
            return dal.UserLogin(loginId, loginPwd);
        }

        /// <summary>
        /// 根据用户id判断用户是否可用
        /// </summary>
        public Model.User CheckLoginByUserId(string userId)
        {
            return dal.CheckLoginByUserId(userId);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">要取的列名（逗号分开）</param>
        /// <param name="order">排序</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="where">查询条件</param>
        /// <param name="totalCount">总记录数</param>
        public string GetPager(string tableName, string columns, string order, int pageSize, int pageIndex, string where, out int totalCount)
        {
            DataTable dt = ZGZY.Common.SqlPagerHelper.GetPager(tableName, columns, order, pageSize, pageIndex, where, out totalCount);
            dt.Columns.Add(new DataColumn("UserRoleId"));
            dt.Columns.Add(new DataColumn("UserRole"));
            dt.Columns.Add(new DataColumn("UserDepartmentId"));
            dt.Columns.Add(new DataColumn("UserDepartment"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //这么写多连了20*2次数据库，待改进
                DataTable dtrole = new ZGZY.BLL.Role().GetRoleByUserId(Convert.ToInt32(dt.Rows[i]["Id"]));
                DataTable dtdepartment = new ZGZY.BLL.Department().GetDepartmentByUserId(Convert.ToInt32(dt.Rows[i]["Id"]));
                dt.Rows[i]["UserRoleId"] = ZGZY.Common.JsonHelper.ColumnToJson(dtrole, 0);
                dt.Rows[i]["UserRole"] = ZGZY.Common.JsonHelper.ColumnToJson(dtrole, 1);
                dt.Rows[i]["UserDepartmentId"] = ZGZY.Common.JsonHelper.ColumnToJson(dtdepartment, 0);
                dt.Rows[i]["UserDepartment"] = ZGZY.Common.JsonHelper.ColumnToJson(dtdepartment, 1);
            }
            return ZGZY.Common.JsonHelper.ToJson(dt);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        public int AddUser(Model.User user)
        {
            ZGZY.Model.User userCompare = dal.GetUserByUserId(user.UserId);
            if (userCompare != null)
            {
                throw new Exception("已经存在此用户！");
            }
            return dal.AddUser(user);
        }

        /// <summary>
        /// 删除用户（可批量删除，删除用户同时删除对应的权限和所处的部门）
        /// </summary>
        public bool DeleteUser(string idList)
        {
            return dal.DeleteUser(idList);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        public bool EditUser(Model.User user, string originalName)
        {
            if (user.UserId != originalName && dal.GetUserByUserId(user.UserId) != null)
            {
                throw new Exception("已经存在此用户！");
            }
            return dal.EditUser(user);
        }

        /// <summary>
        /// 获取用户信息（“我的信息”）
        /// </summary>
        public string GetUserInfo(int userId)
        {
            DataTable dt = dal.GetUserInfo(userId);

            if (dt.Rows.Count > 1)
            {
                DataView dataView = new DataView(dt);
                DataTable dtDistinctRoleName = dataView.ToTable(true, new string[] { "RoleName" });
                DataTable dtDistinctDepartmentName = dataView.ToTable(true, new string[] { "DepartmentName" });

                string roleNames = "";
                string departmentNames = "";
                for (int i = 0; i < dtDistinctRoleName.Rows.Count; i++)
                {
                    roleNames += dtDistinctRoleName.Rows[i]["RoleName"] + ",";
                }
                for (int j = 0; j < dtDistinctDepartmentName.Rows.Count; j++)
                {
                    departmentNames += dtDistinctDepartmentName.Rows[j]["DepartmentName"] + ",";
                }

                DataTable dtNew = dt.Clone();

                DataRow rowNew = dtNew.NewRow();
                rowNew["UserId"] = dt.Rows[0]["UserId"];
                rowNew["UserName"] = dt.Rows[0]["UserName"];
                rowNew["AddDate"] = dt.Rows[0]["AddDate"];
                rowNew["RoleName"] = roleNames.Trim(',');
                rowNew["DepartmentName"] = departmentNames.Trim(',');
                dtNew.Rows.Add(rowNew);

                return ZGZY.Common.JsonHelper.ToJson(dtNew);
            }
            else
                return ZGZY.Common.JsonHelper.ToJson(dt);
        }

    }
}
