using System.Data;

namespace ZGZY.IDAL
{
    /// <summary>
    /// 权限接口（不同的数据库访问类实现接口达到多数据库的支持）
    /// </summary>
    public interface IAuthority
    {
        /// <summary>
        /// 判断当前用户是否有权限
        /// </summary>
        /// <param name="menuCode">菜单标识码</param>
        /// <param name="buttonCode">按钮标识码</param>
        /// <param name="userId">用户主键</param>
        bool IfAuthority(string menuCode, string buttonCode, int userId);

    }
}
