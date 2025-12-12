using DH.Entity;

using NewLife;
using NewLife.Common;
using NewLife.Log;

using Pek.Configs;
using Pek.Events;
using Pek.Infrastructure;
using Pek.NCubeUI;
using Pek.NCubeUI.Areas.Admin;
using Pek.NCubeUI.Events;
using Pek.PearUI.Areas.Admin.Controllers;
using Pek.VirtualFileSystem;

using XCode;
using XCode.Membership;

namespace Pek.PearUI;

/// <summary>
/// 表示应用程序启动时配置服务和中间件的对象
/// </summary>
public class DHCubeStartup : IPekStartup {
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
                modelDetail = new UserDetail
                {
                    Id = model.ID,
                    TenantId = 1,
                    DepartmentIds = ",1,"
                };
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
        if (!PekSysSetting.Current.IsInstalled)
        {
            var _eventPublisher = NewLife.Model.ObjectContainer.Provider.GetPekService<IEventPublisher>();
            // 系统安装生成
            _eventPublisher?.Publish(new InstallEvent());
            XTrace.WriteLine($"调测1：{PekSysSetting.Current.IsInstalled}：{_eventPublisher == null}");

            var list = new List<Role>();

            var modelrole = Role.FindByID(1);
            modelrole.Name = "超级管理员";
            list.Add(modelrole);

            var modelRoleEx = new RoleEx();
            modelRoleEx.Id = modelrole.ID;
            modelRoleEx.IsAdmin = true;
            modelRoleEx.Insert();

            modelrole = Role.FindByID(2);
            modelrole.Name = "管理员";
            modelrole.Remark = "普通管理员拥有全部的管理权限";
            modelrole.IsSystem = true;
            list.Add(modelrole);

            modelRoleEx = new RoleEx();
            modelRoleEx.Id = modelrole.ID;
            modelRoleEx.IsAdmin = true;
            modelRoleEx.Insert();

            modelrole = Role.FindByID(3);
            modelrole.Name = "高级用户";
            modelrole.Remark = "业务管理人员，可以管理业务模块，可以分配授权用户等级";
            if (!DHSetting.Current.IsOnlyManager)
            {
                modelrole.IsSystem = true;
            }
            list.Add(modelrole);

            modelrole = Role.FindByID(4);
            modelrole.Name = "普通用户";
            modelrole.Remark = "普通业务人员，可以使用系统常规业务模块功能";
            if (!DHSetting.Current.IsOnlyManager)
            {
                modelrole.IsSystem = true;
            }
            list.Add(modelrole);

            modelrole = Role.FindByID(5);
            if (modelrole == null)
            {
                modelrole = new Role();
                modelrole.ID = 5;
            }

            modelrole.Name = "默认用户";
            modelrole.Remark = "新注册用户默认属于默认用户组";
            if (!DHSetting.Current.IsOnlyManager)
            {
                modelrole.IsSystem = true;
            }
            list.Add(modelrole);

            list.Save();

            var departmentList = Department.FindAllWithCache().OrderBy(e => e.ID);
            foreach (var item in departmentList)
            {
                if (item.ParentID > 0)
                {
                    item.Level = 1;
                    item.Ex4 = $"{item.Parent?.Ex4}{item.ID},";
                }
                else
                {
                    item.Ex4 = $",{item.ID},";
                }
                item.Update();
                Department.Meta.Cache.Clear("", true);
            }

            PekSysSetting.Current.IsInstalled = true;
            PekSysSetting.Current.Save();
        }
    }

    /// <summary>
    /// 调整菜单
    /// </summary>
    public void ChangeMenu()
    {

    }

    /// <summary>
    /// 注册虚拟文件路径
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureVirtualFileSystem(DHVirtualFileSystemOptions options)
    {
        options.FileSets.AddEmbedded<DHCubeStartup>(typeof(DHCubeStartup).Namespace);
        // options.FileSets.Add(new EmbeddedFileSet(item.Assembly, item.Namespace));
    }

    /// <summary>
    /// 注册路由
    /// </summary>
    /// <param name="endpoints">路由生成器</param>
    public void UseDHEndpoints(IEndpointRouteBuilder endpoints)
    {
    }

    /// <summary>
    /// 升级更新逻辑
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
    public Int32 StartupOrder => 99;

    /// <summary>
    /// 获取此启动配置实现的顺序。主要针对ConfigureMiddleware、UseRouting前执行的数据、UseAuthentication或者UseAuthorization后面 Endpoints前执行的数据
    /// </summary>
    public Int32 ConfigureOrder => 0;
}
