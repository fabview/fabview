using System;
using System.Collections.Generic;
using System.Text;

using log4net;

//Blog：oppoic.cnblogs.com
//QQ群：33353329

namespace ZGZY.DALFactory
{
    /// <summary>
    /// 工厂类：创建访问数据库的实例对象
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// 根据传入的类名获取实例对象
        /// </summary>
        private static object GetInstance(string name)
        {
            //ILog log = LogManager.GetLogger(typeof(Factory));  //初始化日志记录器

            string configName = System.Configuration.ConfigurationManager.AppSettings["DataAccess"];
            if (string.IsNullOrEmpty(configName))
            {
                //log.Fatal("没有从配置文件中获取命名空间名称！");   //Fatal致命错误，优先级最高
                throw new InvalidOperationException();    //抛错，代码不会向下执行了
            }

            string className = string.Format("{0}.{1}", configName, name);  //ZGZY.SQLServerDAL.传入的类名name

            //加载程序集
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(configName);
            //创建指定类型的对象实例
            return assembly.CreateInstance(className);
        }

        /// <summary>
        /// 利用反射获取访问登录信息的数据访问对象（结合配置文件app.config）
        /// </summary>
        public static ZGZY.IDAL.IAuthority GetAuthorityDAL()
        {
            ZGZY.IDAL.IAuthority authority = GetInstance("Authority") as ZGZY.IDAL.IAuthority;
            return authority;
        }

        public static ZGZY.IDAL.IBug GetBugDAL()
        {
            ZGZY.IDAL.IBug bug = GetInstance("Bug") as ZGZY.IDAL.IBug;
            return bug;
        }

        public static ZGZY.IDAL.IButton GetButtonDAL()
        {
            ZGZY.IDAL.IButton button = GetInstance("Button") as ZGZY.IDAL.IButton;
            return button;
        }
        public static ZGZY.IDAL.IDepartment GetDepartmentDAL()
        {
            ZGZY.IDAL.IDepartment department = GetInstance("Department") as ZGZY.IDAL.IDepartment;
            return department;
        }

        public static ZGZY.IDAL.ILoginLog GetLoginInfoDAL()
        {
            ZGZY.IDAL.ILoginLog loginInfo = GetInstance("LoginLog") as ZGZY.IDAL.ILoginLog;
            return loginInfo;
        }

        public static ZGZY.IDAL.IMenu GetMenuDAL()
        {
            ZGZY.IDAL.IMenu menu = GetInstance("Menu") as ZGZY.IDAL.IMenu;
            return menu;
        }

        public static ZGZY.IDAL.IRole GetRoleDAL()
        {
            ZGZY.IDAL.IRole role = GetInstance("Role") as ZGZY.IDAL.IRole;
            return role;
        }

        public static ZGZY.IDAL.IRoleMenuButton GetRoleMenuButtonDAL()
        {
            ZGZY.IDAL.IRoleMenuButton roleMenuButton = GetInstance("RoleMenuButton") as ZGZY.IDAL.IRoleMenuButton;
            return roleMenuButton;
        }

        public static ZGZY.IDAL.IUser GetUserDAL()
        {
            ZGZY.IDAL.IUser user = GetInstance("User") as ZGZY.IDAL.IUser;
            return user;
        }

        public static ZGZY.IDAL.IUserDepartment GetUserDepartmentDAL()
        {
            ZGZY.IDAL.IUserDepartment userDepartment = GetInstance("UserDepartment") as ZGZY.IDAL.IUserDepartment;
            return userDepartment;
        }

        public static ZGZY.IDAL.IUserOperateLog GetUserOperateLogDAL()
        {
            ZGZY.IDAL.IUserOperateLog userOperateLog = GetInstance("UserOperateLog") as ZGZY.IDAL.IUserOperateLog;
            return userOperateLog;
        }

        public static ZGZY.IDAL.IUserRole GetUserRoleDAL()
        {
            ZGZY.IDAL.IUserRole userRole = GetInstance("UserRole") as ZGZY.IDAL.IUserRole;
            return userRole;
        }

    }
}
