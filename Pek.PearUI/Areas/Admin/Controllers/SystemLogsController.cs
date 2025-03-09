using DG.Web.Framework;

using Microsoft.AspNetCore.Mvc;

using Pek;
using Pek.IO;
using Pek.Models;

using System.ComponentModel;

using XCode.Membership;

using YRY.Web.Controllers.Areas.Admin;
using YRY.Web.Controllers.Common;

namespace DH.Cube.Areas.Admin.Controllers;

/// <summary>系统日志</summary>
[DisplayName("系统日志")]
[Description("系统日志的管理")]
[AdminArea]
[DHMenu(49, ParentMenuName = "System", CurrentMenuUrl = "~/{area}/SystemLogs", CurrentMenuName = "SystemLogs", LastUpdate = "20240124")]
public class SystemLogsController : BaseAdminControllerX {
    /// <summary>菜单顺序。扫描是会反射读取</summary>
    protected static Int32 MenuOrder { get; set; } = 49;

    /// <summary>
    /// 系统日志
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("系统日志")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// 系统日志列表
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("系统日志列表")]
    public IActionResult GetList(Int32 page, Int32 limit)
    {
        var DirectoryPath = "Log".AsDirectory();

        var list = FileSystemObject.GetDirectoryAllInfos(DirectoryPath.FullName, FsoMethod.All).OrderByDescending(e => e.creatime);

        return Json(new
        {
            code = 0,
            msg = "success",
            count = list.Count(),
            images = list.Select(e =>
            {
                var type = String.Empty;
                if (e.type == 1)
                {
                    type = "directory";
                }
                else
                {
                    type = e.content_type;
                }

                //if (type.Contains("png", StringComparison.OrdinalIgnoreCase) || type.Contains("gif", StringComparison.OrdinalIgnoreCase) || type.Contains("png", StringComparison.OrdinalIgnoreCase))
                //{

                //}

                var path = e.rname.Replace(DirectoryPath.FullName, "Log").Replace("\\", "/");

                return new { name = e.name, type = type, path = path, iconurl = "/libs/pear/module/fileManager/ico/txt.png" };
            })
        });
    }

    /// <summary>
    /// 系统日志列表
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("系统日志列表")]
    public IActionResult GetInfo(String Path)
    {
        var File = Path.AsFile();

        var Content = File.ReadString(true);

        ViewBag.Content = Content;
        return View();
    }
}