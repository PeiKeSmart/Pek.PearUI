using System.ComponentModel;
using System.Dynamic;

using DH.Core.Domain.Localization;
using DH.Entity;

using Microsoft.AspNetCore.Mvc;

using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Serialization;

using Newtonsoft.Json;

using Pek;
using Pek.Helpers;
using Pek.Models;
using Pek.NCubeUI.Areas.Admin;
using Pek.NCubeUI.Common;
using Pek.PearUI.Common;
using Pek.Webs;

using XCode.Membership;

namespace Pek.PearUI.Areas.Admin.Controllers;

/// <summary>
/// App版本
/// </summary>
[DisplayName("App版本")]
[Description("App版本管理")]
[AdminArea]
[DHMenu(77, ParentMenuName = "System", CurrentMenuUrl = "~/{area}/AppVersion", CurrentMenuName = "AppVersionList", LastUpdate = "20240522")]
public class AppVersionController : PekCubeAdminControllerX {
    /// <summary>菜单顺序。扫描是会反射读取</summary>
    protected static Int32 MenuOrder { get; set; } = 77;

    /// <summary>
    /// App版本
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Detail)]
    [DisplayName("App版本")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>App版本列表接口</summary>
    /// <returns></returns>
    [DisplayName("App版本管理")]
    [EntityAuthorize(PermissionFlags.Detail)]
    public IActionResult GetList(String FileName, Int32 AType, Int32 page, Int32 limit)
    {
        var pages = new PageParameter
        {
            PageIndex = page,
            PageSize = limit,
            RetrieveTotalCount = true,
            Sort = AppVersion._.Id,
            Desc = true
        };
        var List = AppVersion.Search(FileName, pages, AType);
        var data = List.Select(e =>
        {
            return new
            {
                e.Id,
                e.AType,
                e.Version,
                Content = AppVersionLan.FindByAIdAndLId(e.Id, WorkingLanguage.Id, true),
                e.FilePath,
                e.CstFilepath,
                e.ForeignCstFilepath,
                e.IsQiangZhi,
                e.UpdateUser,
                e.UpdateTime,
                e.UpdateIP,
                e.FileName,
                e.Size,
                e.BoundId,
            };
        });

        return Json(new { code = 0, msg = "", count = pages.TotalCount, data });
    }

    /// <summary>上传App</summary>
    /// <returns></returns>
    [DisplayName("上传App")]
    [EntityAuthorize(PermissionFlags.Insert)]
    public IActionResult UpgradeDetail()
    {
        ViewBag.LanguageList = Language.FindByStatus().OrderBy(e => e.DisplayOrder);
        return View();
    }

    /// <summary>
    /// 上传App
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [DisplayName("上传App")]
    [EntityAuthorize(PermissionFlags.Insert)]
    public IActionResult UploadFiles(IFormFile file)
    {
        var res = new DResult();

        if (file == null)
        {
            XTrace.WriteLine("获取到的文件为空");

            res.msg = GetResource("文件上传有误");
            return Json(res);
        }

        var FileName = file.FileName;

        var Version = String.Empty;
        if (FileName.Contains('(', StringComparison.OrdinalIgnoreCase) && FileName.Contains(')', StringComparison.OrdinalIgnoreCase))
        {
            var start = FileName.IndexOf('(');
            var end = FileName.IndexOf(')');
            Version = FileName[(start + 1)..end];
        }

        dynamic obj = new ExpandoObject();
        obj.OriginFileName = FileName;
        obj.FileSize = file.Length;

        var filename = $"{Pek.Helpers.Randoms.RandomStr(13).ToLower()}{Path.GetExtension(FileName)}";
        var filepath = DHSetting.Current.UploadPath.CombinePath($"AppVersion/{filename}");

        var saveFileName = filepath.GetFullPath();
        var f = saveFileName.AsFile();
        if (f.Exists)
        {
            f.Delete();
        }
        saveFileName.EnsureDirectory();
        file.SaveAs(saveFileName);

        obj.FileUrl = filepath.Replace("\\", "/");

        var model = new UploadInfo();
        model.Content = JsonConvert.SerializeObject(obj);
        model.Insert();

        res.msg = GetResource("文件上传成功");
        res.success = true;
        res.data = new { model.Id };
        res.extdata = new { Version };
        return Json(res);
    }

    /// <summary>
    /// 新增保存
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [DisplayName("新增保存")]
    [EntityAuthorize(PermissionFlags.Insert)]
    public IActionResult UpgradeDetails(int UpdateId, int AType, string Version, string Content, bool IsQiangZhi, string FileName, string CstFilepath, string ForeignCstFilepath, String BoundId)
    {
        var result = new DResult();

        UploadInfo? Model = null;
        IDictionary<String, Object?>? dic = null;
        if (UpdateId > 0)
        {
            Model = UploadInfo.FindById(UpdateId);
            if (Model == null)
            {
                result.msg = GetResource($"文件上传有误");
                return Json(result);
            }

            dic = JsonParser.Decode(Model.Content!);
        }

        var model = new AppVersion
        {
            AType = AType,
            Version = Version,
            Content = Content,
            IsQiangZhi = IsQiangZhi,
            CstFilepath = CstFilepath,
            ForeignCstFilepath = ForeignCstFilepath,
            BoundId = BoundId
        };
        if (dic != null)
        {
            model.FileName = dic["OriginFileName"].SafeString();
            model.Size = dic["FileSize"].ToDGInt();
            model.FilePath = dic["FileUrl"].SafeString();

            Model?.Delete();
        }
        else
        {
            model.FileName = FileName;
        }

        if (model.FilePath.IsNullOrWhiteSpace() && model.CstFilepath.IsNullOrWhiteSpace() && model.ForeignCstFilepath.IsNullOrWhiteSpace())
        {
            result.msg = GetResource("请上传文件或者填入国内/国外的下载链接");
            return Json(result);
        }
        model.Insert();

        var localizationSettings = LocalizationSettings.Current;
        if (localizationSettings.IsEnable)
        {
            var Languagelist = Language.FindByStatus().OrderBy(e => e.DisplayOrder); //获取全部有效语言
            var list = AppVersionLan.FindAllByAId(model.Id);                         //获取到的当前App的语言
            using (var tran1 = AppVersionLan.Meta.CreateTrans())
            {
                foreach (var item in Languagelist)
                {
                    var ex = list.Find(x => x.LId == item.Id);
                    if (ex != null)
                    {
                        ex.Content = GetRequest($"[{item.Id}].Content");
                        ex.Update();
                    }
                    else
                    {
                        ex = new AppVersionLan();
                        ex.Content = GetRequest($"[{item.Id}].Content");
                        ex.LId = item.Id;
                        ex.AId = model.Id;
                        ex.Insert();
                    }
                }
                tran1.Commit();
            }
        }

        result.success = true;
        result.msg = GetResource("保存成功");
        return Json(result);
    }

    /// <summary>删除App文件</summary>
    /// <returns></returns>
    [DisplayName("删除App文件")]
    [EntityAuthorize(PermissionFlags.Delete)]
    public IActionResult Delete(Int32 Id)
    {
        var result = new DResult();

        if (Id <= 0)
        {
            result.msg = GetResource("数据有误");
            return Json(result);
        }

        var model = AppVersion.FindById(Id);
        if (model == null)
        {
            result.msg = GetResource("数据不存在");
            return Json(result);
        }

        var saveFileName = model.FilePath?.GetFullPath();
        if (!saveFileName.IsNullOrWhiteSpace())
        {
            var file = saveFileName.AsFile();
            if (file.Exists)
            {
                file.Delete();
            }
        }

        model.Delete();

        result.success = true;
        result.msg = GetResource("删除成功");
        return Json(result);
    }

    /// <summary>
    /// 编辑
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    [DisplayName("编辑")]
    [EntityAuthorize(PermissionFlags.Update)]
    public IActionResult Edit(Int32 Id)
    {
        var model = AppVersion.FindById(Id);

        if (model == null)
        {
            return Content(GetResource("数据不存在"));
        }

        ViewBag.LanguageList = Language.FindByStatus().OrderBy(e => e.DisplayOrder);
        return View(model);
    }

    /// <summary>编辑</summary>
    /// <returns></returns>
    [HttpPost]
    [DisplayName("编辑")]
    [EntityAuthorize(PermissionFlags.Update)]
    public IActionResult Edit(int UpdateId, int AType, string Version, string Content, bool IsQiangZhi, string CstFilepath, string ForeignCstFilepath, int FileId, String BoundId)
    {
        var result = new DResult();

        if (UpdateId <= 0)
        {
            result.msg = GetResource("数据有误");
            return Json(result);
        }

        var Model = AppVersion.FindById(UpdateId);
        if (Model == null)
        {
            result.msg = GetResource("数据不存在");
            return Json(result);
        }

        if (FileId > 0)
        {
            var ModelUploadInfo = UploadInfo.FindById(FileId);
            if (ModelUploadInfo == null)
            {
                result.msg = GetResource($"请重新上传");
                return Json(result);
            }

            var dic = JsonParser.Decode(ModelUploadInfo.Content);
            if (dic == null)
            {
                result.msg = GetResource($"请重新上传");
                return Json(result);
            }

            Model.FileName = dic["OriginFileName"].SafeString();
            Model.Size = dic["FileSize"].ToDGInt();
            Model.FilePath = dic["FileUrl"].SafeString();

            ModelUploadInfo.Delete();
        }

        Model.AType = AType;
        Model.Content = Content;
        Model.BoundId = BoundId;

        var localizationSettings = LocalizationSettings.Current;
        if (localizationSettings.IsEnable)
        {
            var Languagelist = Language.FindByStatus().OrderBy(e => e.DisplayOrder); //获取全部有效语言

            var list = AppVersionLan.FindAllByAId(Model.Id);  //获取到的当前App的语言
            using (var tran1 = AppVersionLan.Meta.CreateTrans())
            {
                foreach (var item in Languagelist)
                {
                    var ex = list.Find(x => x.LId == item.Id);
                    if (ex != null)
                    {
                        ex.Content = GetRequest($"[{item.Id}].Content");
                        ex.Update();
                    }
                    else
                    {
                        ex = new AppVersionLan();
                        ex.Content = GetRequest($"[{item.Id}].Content");
                        ex.LId = item.Id;
                        ex.AId = Model.Id;
                        ex.Insert();
                    }
                }
                tran1.Commit();
            }
        }

        if (!Version.IsNullOrWhiteSpace())
        {
            Model.Version = Version;
        }

        Model.IsQiangZhi = IsQiangZhi;
        Model.CstFilepath = CstFilepath;
        Model.ForeignCstFilepath = ForeignCstFilepath;
        Model.Update();

        result.success = true;
        result.msg = GetResource("保存成功");
        return Json(result);
    }

    /// <summary>
    /// 日志管理
    /// </summary>
    /// <returns></returns>
    [EntityAuthorize((PermissionFlags)16)]
    [DisplayName("日志管理")]
    public IActionResult ManageAppVersionLogs(Int32 Id)
    {
        var model = AppVersion.FindById(Id);
        if (model == null)
        {
            return Content(GetResource("数据不存在"));
        }

        return View(model);
    }

    /// <summary>
    /// APP各版本日志管理
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [EntityAuthorize((PermissionFlags)16)]
    [DisplayName("日志管理")]
    public IActionResult ManageAppVersionLog(string Version, Int32 page, Int32 limit)
    {
        var pages = new PageParameter
        {
            PageIndex = page,
            PageSize = limit,
            RetrieveTotalCount = true,
            Sort = AppVersionLog._.Id,
            Desc = true
        };
        var List = AppVersionLog.SearchVersion(Version, pages);
        var data = List.Select(e =>
        {
            return new
            {
                e.Id,
                e.Os,
                e.Version,
                e.BoundId,
                e.CreateUser,
                e.CreateTime,
                e.CreateIP
            };
        });

        return Json(new { code = 0, msg = "", count = pages.TotalCount, data });
    }
}