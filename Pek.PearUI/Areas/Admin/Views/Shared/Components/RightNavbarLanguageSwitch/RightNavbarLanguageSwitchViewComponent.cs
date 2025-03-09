using DH.Core;
using DH.Core.Infrastructure;
using DH.Entity;
using DH.Services.Components;

using Microsoft.AspNetCore.Mvc;

namespace DH.Cube.Areas.Admin.Views.Shared.Components.RightNavbarLanguageSwitch;

/// <summary>
/// 顶部翻译
/// </summary>
public class RightNavbarLanguageSwitchViewComponent : DHViewComponent {
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