using Microsoft.AspNetCore.Mvc.Filters;

using NewLife.Web;
using Pek.NCube.BaseControllers;
using Pek.Webs;

namespace Pek.PearUI.Common;

/// <summary>
/// 后台控制器基类
/// </summary>
public class PekCubeAdminControllerX : PekAdminControllerBaseX
{
    /// <summary>动作执行前</summary>
    /// <param name="context"></param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        // Ajax请求不需要设置ViewBag
        if (!Request.IsAjaxRequest())
        {
            // 默认加上分页给前台
            var ps = context.ActionArguments.ToNullable();  // ActionArguments取的是Action中定义过的参数，未定义的参数取不到
            var p = ps["p"] as Pager ?? new Pager();

            HttpContext.Items["PekPage"] = p;
        }
        else
        {

        }
    }
}
