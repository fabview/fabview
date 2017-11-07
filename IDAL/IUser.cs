using System.Collections.Generic;
using System.Data;

namespace ZGZY.IDAL
{
    /// <summary>
    /// 用户接口（不同的数据库访问类实现接口达到多数据库的支持）
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        ZGZY.Model.User GetUserByUserId(string userId);

        /// <summary>
        /// 根据id获取用户
        /// </summary>
        ZGZY.Model.User GetUserById(string id);

        /// <summary>
        /// 首次登陆强制修改密码
        /// </summary>
        bool InitUserPwd(ZGZY.Model.User user);

        /// <summary>
        /// 修改密码
        /// </summary>
        bool ChangePwd(ZGZY.Model.User user);

        /// <summary>
        /// 用户登录
        /// </summary>
        ZGZY.Model.User UserLogin(string loginId, string loginPwd);

        /// <summary>
        /// 根据用户id判断用户是否可用
        /// </summary>
        ZGZY.Model.User CheckLoginByUserId(string userId);

        /// <summary>
        /// 添加用户
        /// </summary>
        int AddUser(ZGZY.Model.User user);

        /// <summary>
        /// 删除用户（可批量删除，删除用户同时删除对应的：角色/权限/部门）
        /// </summary>
        bool DeleteUser(string idList);

        /// <summary>
        /// 修改用户
        /// </summary>
        bool EditUser(ZGZY.Model.User user);

        /// <summary>
        /// 获取用户信息（“我的信息”）
        /// </summary>
        DataTable GetUserInfo(int userId);

    }
}
