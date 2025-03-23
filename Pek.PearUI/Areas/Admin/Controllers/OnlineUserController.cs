using System.ComponentModel;

using DH.Entity;

using Microsoft.AspNetCore.Mvc;

using NewLife;
using NewLife.Data;

using Pek.Models;
using Pek.NCubeUI.Areas.Admin;
using Pek.NCubeUI.Common;
using Pek.PearUI.Common;

using XCode.Membership;

namespace Pek.PearUI.Areas.Admin.Controllers;

/// <summary>
/// 在线用户管理
/// </summary>
[DisplayName("在线用户")]
[Description("用于管理一定时间内有操作的用户数据")]
[AdminArea]
[DHMenu(65, ParentMenuName = "DHUser", CurrentMenuUrl = "~/{area}/OnlineUser", CurrentMenuName = "OnlineUserList", LastUpdate = "20240124")]
public class OnlineUserController : PekCubeAdminControllerX {

    /// <summary>菜单顺序。扫描是会反射读取</summary>
    protected static Int32 MenuOrder { get; set; } = 65;

    /// <summary>
    /// 在线用户
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("在线用户")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// 根据条件搜索
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("根据条件搜索")]
    public IActionResult GetPageList(Int32 page = 1, Int32 limit = 10)
    {
        var pages = new PageParameter
        {
            PageIndex = page,
            PageSize = limit,
            RetrieveTotalCount = true,
            Sort = UserOnline._.ID,
            Desc = true,
        };
        var list = UserOnline.Search(null!, pages).Select(e =>
        {
            return new
            {
                e.ID,
                e.UserID,
                e.Name,
                e.SessionID,
                e.OAuthProvider,
                e.Times,
                e.Page,
                e.Platform,
                e.OS,
                e.Device,
                e.Brower,
                e.NetType,
                e.DeviceId,
                e.Status,
                e.OnlineTime,
                e.LastError,
                e.Address,
                e.TraceId,
                e.CreateIP,
                CreateTime = e.CreateTime.ToFullString(),
                e.UpdateIP,
                UpdateTime = e.UpdateTime.ToFullString(),
            };
        });

        return Json(new { code = 0, msg = "success", count = pages.TotalCount, data = list });
    }
}