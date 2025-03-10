using DH.Entity;

using Microsoft.AspNetCore.Mvc;

using Pek.NCube;
using Pek.NCubeUI.Components;

namespace Pek.PearUI.Areas.Admin.Views.Shared.Components.RightNavbarLanguageSwitch;

/// <summary>
/// 顶部翻译
/// </summary>
public class RightNavbarLanguageSwitchViewComponent : PekViewComponent {
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var _workContext = EngineContext.Current.Resolve<IWorkContext>();

        var model = new RightNavbarLanguageSwitchViewModel
        {
            CurrentLanguage = Pek.Webs.HttpContext.Current.Items["WorkingLanguage"] as Language,
            Languages = Language.FindByStatus(true).OrderBy(x => x.Id)
        };

        return await Task.FromResult(View(model)).ConfigureAwait(false);
    }
}