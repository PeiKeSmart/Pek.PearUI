using Microsoft.AspNetCore.Mvc;

using NewLife.Model;

using Pek.Events;
using Pek.Infrastructure;
using Pek.NCubeUI.Components;
using Pek.NCubeUI.Events;

namespace Pek.PearUI.Components;

/// <summary>
/// 小部件视图组件
/// </summary>
public partial class WidgetViewComponent : PekViewComponent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="WidgetZone"></param>
    /// <param name="AdditionalData"></param>
    /// <returns></returns>
    public async Task<IViewComponentResult> InvokeAsync(String WidgetZone, object? AdditionalData = null)
    {
        var eventPublisher = ObjectContainer.Provider.GetPekService<IEventPublisher>();

        var widgetEvent = new WidgetEvent(WidgetZone);
        await eventPublisher!.PublishAsync(widgetEvent).ConfigureAwait(false);

        if (widgetEvent.Data == null || !widgetEvent.Data.Any()) return Content("");

        return View(widgetEvent.Data);
    }
}