using DH.Cube.Areas.Admin.Controllers;
using DH.Entity;

using NewLife;
using NewLife.Common;

using Pek.Infrastructure;
using Pek.NCubeUI;
using Pek.NCubeUI.Areas.Admin;
using Pek.VirtualFileSystem;

using XCode.Membership;

namespace Pek.PearUI;

/// <summary>
/// 表示应用程序启动时配置服务和中间件的对象
/// </summary>
public class CubeStartup : IPekStartup {
    /// <summary>
    /// 配置添加的中间件的使用
    /// </summary>
    /// <param name="application"></param>
    public void Configure(IApplicationBuilder application)
    {
        // 修正系统名，确保可运行
        var set = SysConfig.Current;
        if (set.IsNew || set.Name == "DG.Web.Framework.Views" || set.DisplayName == "DG.Web.Framework.Views")
        {
            set.Name = "DG.Web.Framework";
            set.DisplayName = "创楚平台";
            set.Save();

            var model = User.FindByName("admin");
            if (model != null && model.Password == "21232F297A57A5A743894A0E4A801FC3")
            {
                model.RoleID = 1;
                model.Enable = true;
                model.Password = ManageProvider.Provider?.PasswordProvider.Hash("hlktechcom".MD5());
                model.RegisterTime = DateTime.Now;
                model.RegisterIP = Pek.Helpers.DHWeb.IP;
                model.Ex1 = 1; //1是系统管理员用户
                model.Save();
            }

            var modelDetail = UserDetail.FindById(model!.ID);
            if (modelDetail == null)
            {
                modelDetail = new UserDetail();
                modelDetail.Id = model.ID;
                modelDetail.TenantId = 1;
                modelDetail.DepartmentIds = ",1,";
                modelDetail.Insert();
            }
        }
    }

    /// <summary>
    /// 将区域路由写入数据库
    /// </summary>
    public void ConfigureArea()
    {
        AreaBase.SetRoute<HomeController>(AdminArea.AreaName);
    }

    /// <summary>
    /// 添加并配置任何中间件
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="webHostEnvironment"></param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
    }

    /// <summary>
    /// 注册路由
    /// </summary>
    /// <param name="endpoints">路由生成器</param>
    public void UseDHEndpoints(IEndpointRouteBuilder endpoints)
    {
    }

    /// <summary>
    /// 配置虚拟文件系统
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureVirtualFileSystem(DHVirtualFileSystemOptions options)
    {
        options.FileSets.AddEmbedded<CubeStartup>(typeof(CubeStartup).Namespace);
        // options.FileSets.Add(new EmbeddedFileSet(item.Assembly, item.Namespace));
    }

    /// <summary>
    /// 调整菜单
    /// </summary>
    public void ChangeMenu()
    {

    }

    /// <summary>
    /// 升级处理逻辑
    /// </summary>
    public void Update()
    {

    }

    /// <summary>
    /// 配置使用添加的中间件
    /// </summary>
    /// <param name="application">用于配置应用程序的请求管道的生成器</param>
    public void ConfigureMiddleware(IApplicationBuilder application)
    {

    }

    /// <summary>
    /// UseRouting前执行的数据
    /// </summary>
    /// <param name="application"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void BeforeRouting(IApplicationBuilder application)
    {

    }

    /// <summary>
    /// UseAuthentication或者UseAuthorization后面 Endpoints前执行的数据
    /// </summary>
    /// <param name="application"></param>
    public void AfterAuth(IApplicationBuilder application)
    {

    }

    /// <summary>
    /// 处理数据
    /// </summary>
    public void ProcessData()
    {
    }

    /// <summary>
    /// 获取此启动配置实现的顺序
    /// </summary>
    public int StartupOrder => 101; //常见服务应在错误处理程序之后加载

    /// <summary>
    /// 获取此启动配置实现的顺序。主要针对ConfigureMiddleware、UseRouting前执行的数据、UseAuthentication或者UseAuthorization后面 Endpoints前执行的数据
    /// </summary>
    public Int32 ConfigureOrder => 0;
}
