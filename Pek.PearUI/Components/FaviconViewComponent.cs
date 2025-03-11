using Microsoft.AspNetCore.Mvc;

using NewLife;

using Pek.NCubeUI.Components;
using Pek.PearUI.Models;

namespace Pek.PearUI.Components;

/// <summary>
/// Favicon视图组件
/// </summary>
public partial class FaviconViewComponent : PekViewComponent
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await Task.FromResult(new FaviconAndAppIconsModel
        {
            HeadCode = DHSetting.Current.FaviconAndAppIconsHeadCode
        }).ConfigureAwait(false);

        if (model.HeadCode.IsNullOrEmpty()) return Content("");

        return View(model);
    }
}