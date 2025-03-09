using DG.Web.Framework;

using DH.Entity;
using DH.Services.Services;

using Microsoft.AspNetCore.Mvc;

using NewLife;
using NewLife.Data;

using Pek;
using Pek.Models;

using System.ComponentModel;

using XCode;
using XCode.Membership;

using YRY.Web.Controllers.Areas.Admin;
using YRY.Web.Controllers.Common;

namespace DH.Cube.Areas.Admin.Controllers;

/// <summary>定时作业</summary>
[DisplayName("定时作业")]
[Description("系统定时作业的管理")]
[AdminArea]
[DHMenu(55, ParentMenuName = "System", CurrentMenuUrl = "~/{area}/Jobs", CurrentMenuName = "Jobs", LastUpdate = "20240415")]
public class JobsController : BaseAdminControllerX {
    /// <summary>菜单顺序。扫描是会反射读取</summary>
    protected static Int32 MenuOrder { get; set; } = 55;

    /// <summary>
    /// 定时作业管理
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("定时作业管理")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>定时作业列表</summary>
    /// <returns></returns>
    [DisplayName("定时作业列表")]
    [EntityAuthorize(PermissionFlags.Detail)]
    public IActionResult GetList(String key, Int32 page, Int32 limit)
    {
        key = key.SafeString().Trim();
        var pages = new PageParameter
        {
            PageIndex = page,
            PageSize = limit,
            RetrieveTotalCount = true,
            Sort = CronJob._.Id,
            Desc = true,
        };

        var List = CronJob.Search(key, pages).Select(e => new
        {
            e.Id,
            e.Name,
            e.DisplayName,
            e.Cron,
            e.Method,
            e.Argument,
            e.Enable,
            e.LastTime,
            e.NextTime,
            UpdateUser = UserE.FindByID(e.UpdateUserID)?.Name,
            e.UpdateTime,
            e.UpdateIP,
            e.Remark,
        });
        return Json(new { code = 0, msg = "", count = pages.TotalCount, data = List });
    }

    /// <summary>
    /// 新增定时作业
    /// </summary>
    /// <returns></returns>
    [DisplayName("新增定时作业")]
    [EntityAuthorize(PermissionFlags.Insert)]
    public IActionResult Add()
    {
        return View();
    }

    /// <summary>
    /// 新增定时作业
    /// </summary>
    /// <returns></returns>
    [DisplayName("新增定时作业")]
    [EntityAuthorize(PermissionFlags.Insert)]
    [HttpPost]
    public IActionResult Add(String Name, String DisplayName, String Cron, String Method, String Argument, String Enable, String Remark)
    {
        var res = new DResult();

        if (Name.IsNullOrWhiteSpace())
        {
            res.msg = GetResource("作业名称不能为空");
            return Json(res);
        }

        if (DisplayName.IsNullOrWhiteSpace())
        {
            res.msg = GetResource("显示名不能为空");
            return Json(res);
        }

        if (Cron.IsNullOrWhiteSpace())
        {
            res.msg = GetResource("Cron表达式不能为空");
            return Json(res);
        }

        if (Method.IsNullOrWhiteSpace())
        {
            res.msg = GetResource("命令方法全名不能为空");
            return Json(res);
        }

        var model = CronJob.FindByName(Name);
        if (model != null)
        {
            res.msg = GetResource("作业名称不能重复");
            return Json(res);
        }

        model = new CronJob();
        model.Name = Name;
        model.DisplayName = DisplayName;
        model.Cron = Cron;
        model.Method = Method;
        model.Argument = Argument;
        model.Enable = Enable == "on";
        model.Remark = Remark;

        var next = DateTime.MinValue;
        foreach (var item in model.Cron.Split(";"))
        {
            var cron = new NewLife.Threading.Cron();
            if (!cron.Parse(item)) throw new ArgumentException(GetResource("Cron表达式有误！"), nameof(model.Cron));

            var dt = cron.GetNext(DateTime.Now);
            if (next == DateTime.MinValue || dt < next) next = dt;
        }

        // 重算下一次的时间
        if (model is IEntity e && !e.Dirtys[nameof(model.Name)]) model.NextTime = next;
        JobService.Wake();

        model.Insert();

        res.success = true;
        res.msg = GetResource("新增成功");
        return Json(res);
    }

    /// <summary>
    /// 编辑定时作业
    /// </summary>
    /// <returns></returns>
    [DisplayName("编辑定时作业")]
    [EntityAuthorize(PermissionFlags.Update)]
    public IActionResult Edit(Int32 Id)
    {
        var model = CronJob.FindById(Id);

        if (model == null)
        {
            return Content(GetResource("作业不存在"));
        }

        return View(model);
    }

    /// <summary>
    /// 编辑定时作业
    /// </summary>
    /// <returns></returns>
    [DisplayName("编辑定时作业")]
    [EntityAuthorize(PermissionFlags.Update)]
    [HttpPost]
    public IActionResult Edit(Int32 Id, String Name, String DisplayName, String Cron, String Method, String Argument, String Enable, String Remark)
    {
        var res = new DResult();

        if (Name.IsNullOrWhiteSpace())
        {
            res.msg = GetResource("作业名称不能为空");
            return Json(res);
        }

        if (DisplayName.IsNullOrWhiteSpace())
        {
            res.msg = GetResource("显示名不能为空");
            return Json(res);
        }

        if (Cron.IsNullOrWhiteSpace())
        {
            res.msg = GetResource("Cron表达式不能为空");
            return Json(res);
        }

        if (Method.IsNullOrWhiteSpace())
        {
            res.msg = GetResource("命令方法全名不能为空");
            return Json(res);
        }

        var model = CronJob.FindByName(Name);
        if (model != null && model.Id != Id)
        {
            res.msg = GetResource("作业名称不能重复");
            return Json(res);
        }

        model = CronJob.FindById(Id);
        if (model == null)
        {
            res.msg = GetResource("作业不存在");
            return Json(res);
        }

        model.Name = Name;
        model.DisplayName = DisplayName;
        model.Cron = Cron;
        model.Method = Method;
        model.Argument = Argument;
        model.Enable = Enable == "on";
        model.Remark = Remark;

        var next = DateTime.MinValue;
        foreach (var item in model.Cron.Split(";"))
        {
            var cron = new NewLife.Threading.Cron();
            if (!cron.Parse(item)) throw new ArgumentException(GetResource("Cron表达式有误！"), nameof(model.Cron));

            var dt = cron.GetNext(DateTime.Now);
            if (next == DateTime.MinValue || dt < next) next = dt;
        }

        // 重算下一次的时间
        if (model is IEntity e && !e.Dirtys[nameof(model.Name)]) model.NextTime = next;
        JobService.Wake();

        model.Update();

        res.success = true;
        res.msg = GetResource("编辑成功");
        return Json(res);
    }

    /// <summary>
    /// 删除定时作业
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Delete)]
    [DisplayName("删除定时作业")]
    [HttpPost]
    public IActionResult Delete(Int32 Id)
    {
        var res = new DResult();

        CronJob.Delete(CronJob._.Id == Id);

        res.success = true;
        res.msg = GetResource("删除成功");
        return Json(res);
    }

    /// <summary>
    /// 立即执行
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize((PermissionFlags)16)]
    [DisplayName("立即执行")]
    [HttpGet]
    public IActionResult ExecuteNow(Int32 Id)
    {
        var res = new DResult();

        var entity = CronJob.FindById(Id);
        if (entity != null && entity.Enable)
        {
            entity.NextTime = DateTime.Now;
            entity.Update();

            JobService.Wake(entity.Id, -1);
        }

        res.success = true;
        res.msg = GetResource("已安排执行");
        return Json(res);
    }

    /// <summary>修改状态</summary>
    /// <returns></returns>
    [DisplayName("修改状态")]
    [HttpPost]
    [EntityAuthorize(PermissionFlags.Update)]
    public IActionResult ModifyState(Int32 Id, Boolean Status)
    {
        var result = new DResult();

        var model = CronJob.FindById(Id);
        if (model == null)
        {
            result.msg = GetResource("状态调整出错");
            return Json(result);
        }

        model.Enable = Status;
        model.Update();

        result.success = true;
        result.msg = GetResource("状态调整成功");

        return Json(result);
    }
}
