using DH.Entity;

using XCode.Membership;

namespace Pek.PearUI.Areas.Admin.Views.Shared.Components.RightNavbarUserArea;

/// <summary>
/// 顶部用户区域
/// </summary>
public class RightNavbarUserAreaViewModel {
    /// <summary>
    /// 用户信息
    /// </summary>
    public IUser? LoginInformations { get; set; }

    /// <summary>
    /// 是否启用多租户
    /// </summary>
    public Boolean IsMultiTenancyEnabled { get; set; }

    /// <summary>
    /// 获取显示的登录名
    /// </summary>
    /// <returns></returns>
    public String GetShownLoginName()
    {
        var userName = LoginInformations!.Name;

        if (!IsMultiTenancyEnabled)
        {
            return userName;
        }

        var userDetail = UserDetail.FindById(LoginInformations.ID);
        if (userDetail == null)
        {
            return ".\\" + userName;
        }

        return userDetail.Tenant == null
            ? ".\\" + userName
            : userDetail.Tenant.TenancyName + "\\" + userName;
    }
}