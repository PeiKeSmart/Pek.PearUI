using DG.Web.Framework;

using DH.Entity;

using Microsoft.AspNetCore.Mvc;

using NewLife;
using NewLife.Data;

using Pek.Models;

using System.ComponentModel;

using XCode.Membership;

using YRY.Web.Controllers.Areas.Admin;
using YRY.Web.Controllers.Common;

namespace DH.Cube.Areas.Admin.Controllers;

/// <summary>
/// 在线用户管理
/// </summary>
[DisplayName("在线用户")]
[Description("用于管理一定时间内有操作的用户数据")]
[AdminArea]
[DHMenu(65, ParentMenuName = "DHUser", CurrentMenuUrl = "~/{area}/OnlineUser", CurrentMenuName = "OnlineUserList", LastUpdate = "20240124")]
public class OnlineUserController : BaseAdminControllerX {

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
            Sort = SysOnlineUsers._.Id,
            Desc = true,
        };
        var list = SysOnlineUsers.Search(null, pages).Select(e =>
        {
            var region = e.Region?.Split(',');
            var regions = new List<String>();
            foreach (var item in region)
            {
                regions.Add(LocaleStringResource.GetResource(item));
            }

            return new
            {
                e.Id,
                e.Uid,
                e.Sid,
                NickName = $"{GetResource(e.NickName)}{(e.Name.Length > 0 ? $"({e.Name})" : "")}",
                e.Ip,
                Region = regions.Join(",") + $"_{GetResource(e.Network)}" + e.Numbers,
                e.Clicks,
                e.UserAgent,
                e.Updatetime,
                UserName = e.User?.Name
            };
        });

        return Json(new { code = 0, msg = "success", count = pages.TotalCount, data = list });
    }
}