using Microsoft.AspNetCore.Mvc;

using Pek.NCube.Configs;
using Pek.NCubeUI.Components;

using XCode.Membership;

namespace Pek.PearUI.Areas.Admin.Views.Shared.Components.RightNavbarUserArea;

/// <summary>
/// 顶部用户区域
/// </summary>
public class RightNavbarUserAreaViewComponent : PekViewComponent {
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new RightNavbarUserAreaViewModel
        {
            LoginInformations = ManageProvider.User,
            IsMultiTenancyEnabled = MultiTenancyConfig.Current.IsEnabled
        };

        return await Task.FromResult(View(model)).ConfigureAwait(false);
    }
}