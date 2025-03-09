using DG;

using DH.Services.Components;

using Microsoft.AspNetCore.Mvc;

using XCode.Membership;

namespace DH.Cube.Areas.Admin.Views.Shared.Components.RightNavbarUserArea;

/// <summary>
/// 顶部用户区域
/// </summary>
public class RightNavbarUserAreaViewComponent : DHViewComponent {
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