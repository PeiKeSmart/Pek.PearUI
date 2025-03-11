using Microsoft.AspNetCore.Mvc;
using Pek.NCubeUI.Components;

namespace Pek.PearUI.Components;

/// <summary>
/// 新闻Rss头部链接视图组件
/// </summary>
public partial class NewsRssHeaderLinkViewComponent : PekViewComponent
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IViewComponentResult Invoke()
    {
        if (!DHSetting.Current.ShowNewsHeaderRssUrl)
            return Content("");

        return View();
    }
}
