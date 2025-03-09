using DH.Services.Components;

using Microsoft.AspNetCore.Mvc;

namespace DH.Cube.Areas.Admin.Views.Shared.Components.SideBarMenu;

/// <summary>
/// 左侧菜单
/// </summary>
public class SideBarMenuViewComponent : DHViewComponent {
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var menus = GetMenu().Where(e => !e.FullName.Contains("NewLife.", StringComparison.OrdinalIgnoreCase) && !e.FullName.Contains("IoTWeb.", StringComparison.OrdinalIgnoreCase)).ToList();
        return await Task.FromResult(View(menus)).ConfigureAwait(false);
    }
}