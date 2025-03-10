using Microsoft.AspNetCore.Mvc;

using Pek.NCubeUI.Components;

namespace Pek.PearUI.Areas.Admin.Views.Shared.Components.SideBarMenu;

/// <summary>
/// 左侧菜单
/// </summary>
public class SideBarMenuViewComponent : PekViewComponent {
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var menus = GetMenu().Where(e => !e.FullName.Contains("NewLife.", StringComparison.OrdinalIgnoreCase) && !e.FullName.Contains("IoTWeb.", StringComparison.OrdinalIgnoreCase)).ToList();
        return await Task.FromResult(View(menus)).ConfigureAwait(false);
    }
}