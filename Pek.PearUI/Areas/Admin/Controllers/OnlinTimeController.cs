using System.ComponentModel;
using System.Dynamic;

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
/// 在线时间管理
/// </summary>
[DisplayName("在线时间")]
[Description("在线时间管理，管理会员活跃度")]
[AdminArea]
[DHMenu(64, ParentMenuName = "DHUser", CurrentMenuUrl = "~/{area}/OnlinTime", CurrentMenuName = "OnlinTimeList", LastUpdate = "20240124")]
public class OnlinTimeController : PekCubeAdminControllerX {
    /// <summary>菜单顺序。扫描是会反射读取</summary>
    protected static Int32 MenuOrder { get; set; } = 64;

    /// <summary>
    /// 在线时间
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("在线时间")]
    public IActionResult Index()
    {
        dynamic viewModel = new ExpandoObject();

        var Roles = Role.FindAllWithCache().Select(x => new Role { ID = x.ID, Name = RoleLan.FindByRIdAndLId(x.ID, WorkingLanguage.Id, true).Name }).OrderBy(e => e.ID).ToList();
        viewModel.Roles = Roles;

        return View(viewModel);
    }

    /// <summary>
    /// 根据条件搜索
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <param name="StartTime">开始时间</param>
    /// <param name="EndTime">结束时间</param>
    /// <param name="FieldOrder">排序字段，如Id desc等</param>
    /// <param name="RoleId"></param>
    /// <param name="UName"></param>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("根据条件搜索")]
    public IActionResult GetPageList(DateTime? StartTime, DateTime? EndTime, String FieldOrder, String UName, Int32 RoleId, Int32 page = 1, Int32 limit = 10)
    {
        if (FieldOrder.IsNullOrWhiteSpace())
        {
            FieldOrder = "UpdateTime desc";
        }

        var pages = new PageParameter
        {
            PageIndex = page,
            PageSize = limit,
            RetrieveTotalCount = true,
            OrderBy = FieldOrder,
        };

        var list = SysOnlineTime.Searchs(StartTime, EndTime, RoleId, UName, pages).Select(e => new
        {
            Id = e.ID,
            e.UId,
            e.Year,
            e.Month,
            e.MonthTimes,
            e.DayTimes,
            e.Day1,
            e.Day2,
            e.Day3,
            e.Day4,
            e.Day5,
            e.Day6,
            e.Day7,
            e.Day8,
            e.Day9,
            e.Day10,
            e.Day11,
            e.Day12,
            e.Day13,
            e.Day14,
            e.Day15,
            e.Day16,
            e.Day17,
            e.Day18,
            e.Day19,
            e.Day20,
            e.Day21,
            e.Day22,
            e.Day23,
            e.Day24,
            e.Day25,
            e.Day26,
            e.Day27,
            e.Day28,
            e.Day29,
            e.Day30,
            e.Day31,
            e.UpdateTime,
            UserName = e.User?.Name,
            RoleName = RoleLan.FindByRIdAndLId(e.RoleId, WorkingLanguage.Id, true).Name,
        });

        return Json(new { code = 0, msg = "success", count = pages.TotalCount, data = list });
    }
}