using DH.Entity;

using XCode.Membership;

namespace DH.Cube.Areas.Admin.Views.Shared.Components.RightNavbarUserArea;

public class RightNavbarUserAreaViewModel {
    /// <summary>
    /// 用户信息
    /// </summary>
    public IUser? LoginInformations { get; set; }

    /// <summary>
    /// 是否启用多租户
    /// </summary>
    public Boolean IsMultiTenancyEnabled { get; set; }

    public string GetShownLoginName()
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