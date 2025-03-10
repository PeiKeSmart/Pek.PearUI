using System.ComponentModel;

using Microsoft.AspNetCore.Mvc;

using NewLife;

using Pek.IO;
using Pek.Models;
using Pek.NCubeUI.Areas.Admin;
using Pek.NCubeUI.Common;
using Pek.PearUI.Common;

using XCode.Membership;

namespace Pek.PearUI.Areas.Admin.Controllers;

/// <summary>
/// 系统文件管理
/// </summary>
[DisplayName("系统文件")]
[Description("系统文件管理")]
[AdminArea]
[DHMenu(63, ParentMenuName = "System", CurrentMenuUrl = "~/{area}/SiteFile", CurrentMenuName = "SiteFileList", LastUpdate = "20240428")]
public class SiteFileController : PekCubeAdminControllerX {
    /// <summary>菜单顺序。扫描是会反射读取</summary>
    protected static Int32 MenuOrder { get; set; } = 63;

    /// <summary>
    /// 系统文件管理
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("系统文件管理")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>系统文件列表</summary>
    /// <param name="Path">文件夹路径。默认为空，即根目录</param>
    /// <returns></returns>
    [DisplayName("系统文件列表")]
    [EntityAuthorize(PermissionFlags.Detail)]
    public IActionResult GetList(String Path)
    {
        DirectoryInfo DirectoryPath;
        if (Path.IsNullOrWhiteSpace())
        {
            var dir = AppContext.BaseDirectory;
            DirectoryPath = dir.AsDirectory();
        }
        else
        {
            DirectoryPath = Path.AsDirectory();
        }

        var list = FileSystemObject.GetDirectoryInfos(DirectoryPath.FullName, FsoMethod.All).OrderBy(e => e.type).ThenBy(e => e.name);

        return Json(new
        {
            code = 0,
            msg = "success",
            count = list.Count(),
            images = list.Select(e =>
            {
                return e;
            })
        });
    }
}