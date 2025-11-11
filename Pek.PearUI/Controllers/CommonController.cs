using Microsoft.AspNetCore.Mvc;

using NewLife;

using Pek.Mime;
using Pek.MVC;
using Pek.NCube.BaseControllers;
using Pek.NCubeUI;
using Pek.PearUI.Models;
using Pek.Swagger;

using XCode.Membership;

namespace Pek.PearUI.Controllers;

/// <summary>
/// 公共接口
/// </summary>
[Produces("application/json")]
[CustomRoute(ApiVersions.V1)]
[DHAuthorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class CommonController : PekBaseControllerX
{
    /// <summary>
    /// 获取未读通知数量
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetHeadMessage")]
    public IActionResult GetHeadMessage()
    {
        var list = new List<MessageDto>();
        var model = new MessageDto();
        model.id = 1;
        model.title = GetResource("通知");
        model.children = new List<MessageChild>();
        model.children.Add(new MessageChild
        {
            id = 1,
            avatar = "https://gw.alipayobjects.com/zos/rmsportal/ThXAXghbEsBCCSDihZxY.png",
            title = GetResource("欢迎使用本系统"),
            context = GetResource("欢迎使用本系统"),
            form = GetResource("系统"),
            time = GetResource("刚刚")
        }); ;

        list.Add(model);

        return Json(list);
    }

    /// <summary>
    /// 获取用户头像
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetProfilePicture")]
    public FileResult GetProfilePicture()
    {
        var profilePictureContent = GetUserProfilePicture();
        if (profilePictureContent.IsNullOrWhiteSpace())
        {
            return GetDefaultProfilePictureInternal();
        }

        return File(Convert.FromBase64String(profilePictureContent), MimeTypes.ImagePng);
    }

    private String GetUserProfilePicture()
    {
        var profilePictureContent = String.Empty;

        var user = ManageProvider.User;
        if (user?.Avatar.IsNullOrWhiteSpace() == false)
        {
            var file = user.Avatar.AsFile();
            if (file.Exists)
            {
                profilePictureContent = Convert.ToBase64String(file.ReadBytes());
            }
        }

        return profilePictureContent;
    }

    /// <summary>
    /// 获取默认头像
    /// </summary>
    /// <returns></returns>
    protected FileResult GetDefaultProfilePictureInternal()
    {
        return File(Path.Combine("images", "avatar.jpg"), MimeTypes.ImageJpeg);
    }
}