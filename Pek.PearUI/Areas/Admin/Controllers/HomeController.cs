using DG.Web.Framework;

using DH.Core.Infrastructure;
using DH.Cube.Events.EventModel;
using DH.Services.Membership;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

using NewLife;
using NewLife.Log;

using Pek.Events;
using Pek.Models;

using System.ComponentModel;

using XCode.Membership;

using YRY.Web.Controllers.Areas.Admin;
using YRY.Web.Controllers.Common;

namespace DH.Cube.Areas.Admin.Controllers;

/// <summary>首页</summary>
[DisplayName("首页")]
[Description("后台登录之后的首页")]
[AdminArea]
[DHMenu(100, ParentMenuName = "Home", ParentMenuDisplayName = "工作空间", ParentMenuUrl = "~/{area}/Home/DashBoard", ParentMenuOrder = 100, ParentIcon = "layui-icon-console", CurrentMenuUrl = "~/{area}/Home/DashBoard", CurrentMenuName = "DashBoard", CurrentVisible = false, LastUpdate = "20240124")]
public class HomeController : BaseAdminControllerX {
    /// <summary>菜单顺序。扫描是会反射读取</summary>
    protected static Int32 MenuOrder { get; set; } = 100;

    private readonly IManageProvider _provider;

    static HomeController() => MachineInfo.RegisterAsync();

    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="manageProvider"></param>
    public HomeController(IManageProvider manageProvider)
    {
        _provider = manageProvider;
    }

    [DisplayName("管理后台首页")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        var user = _provider.TryLogin(HttpContext);

        if (user == null || !user.Enable || ManageProvider.User.RoleID == 0) return RedirectToAction("Index", "Login", new { r = Request.GetEncodedPathAndQuery() });

        var _eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        _eventPublisher.Publish(new UserExEvent(user.ID));

        //!!! 租户切换
        var set = DHSetting.Current;
        var tenantId = Request.Query["TenantId"].ToInt(-1);
        if (tenantId >= 0 && set.EnableTenant)
        {
            // 判断租户关系
            var list = TenantUser.FindAllByUserId(user.ID);
            if (list.Any(e => e.TenantId == tenantId) || tenantId == 0)
            {
                var tenant = Tenant.FindById(tenantId);

                XTrace.WriteLine("用户[{0}]切换到租户[{1}/{2}]", user, tenant?.Name ?? "系统管理员", tenant?.Code ?? "0");

                // 切换租户，保存到Cookie
                HttpContext.SaveTenant(tenantId);

                return Redirect($"/{DHSetting.Current.AdminArea}");
            }
        }

        var model = XCode.Membership.Menu.FindByName("Home");
        if (model != null && model.ParentID == 0)
        {
            return LocalRedirect(model.Url);
        }

        return RedirectToAction("DashBoard");
    }

    /// <summary>
    /// 控制台
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("控制台")]
    public IActionResult DashBoard()
    {
        var menus = GetMenu();

        return View(menus);
    }
}