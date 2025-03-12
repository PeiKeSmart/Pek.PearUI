using Microsoft.AspNetCore.Mvc;

using NewLife;
using NewLife.Caching;
using NewLife.Log;

using Pek.Models;
using Pek.NCube;
using Pek.Permissions.Identity.JwtBearer;
using Pek.Security;

namespace Pek.PearUI.Controllers;

/// <summary>
/// 站点控制器
/// </summary>
public class SiteController : PekBaseControllerX
{
    private readonly ICache _cache;

    /// <summary>
    /// Jwt令牌构建器
    /// </summary>
    private IJsonWebTokenBuilder TokenBuilder { get; }

    /// <summary>
    /// 初始化站点控制器
    /// </summary>
    /// <param name="cache">缓存</param>
    /// <param name="tokenBuilder">Jwt令牌构建器</param>
    public SiteController(ICacheProvider cache, IJsonWebTokenBuilder tokenBuilder)
    {
        _cache = cache.Cache;
        TokenBuilder = tokenBuilder;
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
}
