using System.Data;

namespace ZGZY.IDAL
{
    /// <summary>
    /// 按钮接口（不同的数据库访问类实现接口达到多数据库的支持）
    /// </summary>
    public interface IButton
    {
        /// <summary>
        /// 根据菜单标识码和用户id获取此用户拥有该菜单下的哪些按钮权限
        /// </summary>
        DataTable GetButtonByMenuCodeAndUserId(string menuCode, int userId);


    }
}
