using Microsoft.AspNetCore.Mvc;

using Pek.Configs;
using Pek.NCubeUI.Components;

namespace Pek.PearUI.Components;

/// <summary>
/// 博客Rss头部链接视图组件
/// </summary>
public partial class BlogRssHeaderLinkViewComponent : PekViewComponent
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IViewComponentResult Invoke()
    {
        if (!PekSysSetting.Current.ShowHeaderRssUrl)
            return Content("");

        return View();
    }
}