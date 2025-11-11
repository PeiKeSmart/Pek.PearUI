using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using NewLife;
using NewLife.Caching;
using NewLife.Cube;
using NewLife.Cube.ViewModels;
using NewLife.Log;

using Pek.Models;
using Pek.NCube.BaseControllers;
using Pek.Permissions.Identity.JwtBearer;
using Pek.Security;

using XCode.Membership;

namespace Pek.PearUI.Controllers;

/// <summary>
/// 站点控制器
/// </summary>
public class SiteController : PekBaseControllerX
{
    private readonly ICache _cache;
    private readonly IManageProvider _provider;

    /// <summary>
    /// Jwt令牌构建器
    /// </summary>
    private IJsonWebTokenBuilder TokenBuilder { get; }

    /// <summary>
    /// 初始化站点控制器
    /// </summary>
    /// <param name="cache">缓存</param>
    /// <param name="tokenBuilder">Jwt令牌构建器</param>
    /// <param name="provider">管理提供者</param>
    public SiteController(ICacheProvider cache, IJsonWebTokenBuilder tokenBuilder, IManageProvider provider)
    {
        _cache = cache.Cache;
        TokenBuilder = tokenBuilder;
        _provider = provider;
    }

    /// <summary>错误</summary>
    /// <returns></returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var model = HttpContext.Items["Exception"] as ErrorModel;
        if (IsJsonRequest)
        {
            if (model?.Exception != null) return Json(500, null, model.Exception);
        }

        return View("Error", model);
    }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <param name="RefreshToken">刷新令牌</param>
    /// <param name="RefreshExpireMinutes">刷新令牌过期时间</param>
    [HttpPost]
    public IActionResult RefreshToken([FromForm] String RefreshToken, [FromForm] Double RefreshExpireMinutes)
    {
        var result = new DResult();

        if (RefreshToken.IsNullOrWhiteSpace())
        {
            result.msg = GetResource("RefreshToken为空");
            return Json(result);
        }

        var key = $"dh-{RefreshToken}";
        if (_cache.ContainsKey(key))
        {
            result.code = 200;
            result.success = true;
            result.data = new { Token = _cache.Get<JsonWebToken>(key) };

            return Json(result);
        }

        try
        {
            JsonWebToken token = TokenBuilder.Refresh(RefreshToken, 10, RefreshExpireMinutes);
            _cache.Set(key, token, 10);

            result.success = true;
            result.data = token;
            return Json(result);
        }
        catch (Exception ex)
        {
            XTrace.WriteException(ex);
            result.msg = GetResource(ex.Message);
            return Json(result);
        }
    }

    /// <summary>
    /// 获取令牌
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult GetMemberAccessToken()
    {
        var result = new DResult();

        if (ManageProvider.User == null)
        {
            var user = _provider.TryLogin(HttpContext);
            if (user == null)
            {
                result.msg = GetResource("用户未登录");
                return Json(result);
            }
        }

        var payload = new Dictionary<String, String>
        {
            ["clientId"] = ManageProvider.User!.ID.ToString(),
            [ClaimTypes.Sid] = ManageProvider.User.ID.ToString(),
            [ClaimTypes.NameIdentifier] = ManageProvider.User.Name
        };

        var accesstoken = TokenBuilder.Create(payload);
        result.data = accesstoken;
        result.success = true;
        return Json(result);
    }
}
