using DH.Entity;

namespace DH.Cube.Areas.Admin.Views.Shared.Components.RightNavbarLanguageSwitch;

public class RightNavbarLanguageSwitchViewModel {
    public Language? CurrentLanguage { get; set; }

    public IEnumerable<Language>? Languages { get; set; }
}