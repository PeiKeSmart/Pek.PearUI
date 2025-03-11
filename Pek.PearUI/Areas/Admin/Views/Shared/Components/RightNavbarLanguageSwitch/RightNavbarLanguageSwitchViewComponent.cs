using DH.Entity;

using Microsoft.AspNetCore.Mvc;

using Pek.NCube;
using Pek.NCubeUI.Components;

namespace Pek.PearUI.Areas.Admin.Views.Shared.Components.RightNavbarLanguageSwitch;

/// <summary>
/// 顶部翻译
/// </summary>
public class RightNavbarLanguageSwitchViewComponent : PekViewComponent {
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var _workContext = Pek.Webs.HttpContext.Current.RequestServices.GetRequiredService<IWorkContext>();

        var model = new RightNavbarLanguageSwitchViewModel
        {
            CurrentLanguage = Pek.Webs.HttpContext.Current.Items["WorkingLanguage"] as Language,
            Languages = Language.FindByStatus(true).OrderBy(x => x.Id)
        };

        return await Task.FromResult(View(model)).ConfigureAwait(false);
    }
}