using Microsoft.AspNetCore.Mvc;
using Pek.NCubeUI.Components;

namespace Pek.PearUI.Components;

/// <summary>
/// 头部Html标签视图组件
/// </summary>
public class HeadHtmlTagViewComponent : PekViewComponent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="WidgetZone"></param>
    /// <param name="AdditionalData"></param>
    /// <returns></returns>
    public async Task<IViewComponentResult> InvokeAsync(String WidgetZone, object? AdditionalData = null)
    {
        await Task.CompletedTask.ConfigureAwait(false);

        return View();
    }
}