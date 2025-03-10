using DH.Entity;

namespace Pek.PearUI.Areas.Admin.Views.Shared.Components.RightNavbarLanguageSwitch;

/// <summary>
/// 顶部翻译
/// </summary>
public class RightNavbarLanguageSwitchViewModel {
    /// <summary>
    /// 当前语言
    /// </summary>
    public Language? CurrentLanguage { get; set; }

    /// <summary>
    /// 语言列表
    /// </summary>
    public IEnumerable<Language>? Languages { get; set; }
}