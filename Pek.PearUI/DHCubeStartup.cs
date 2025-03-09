﻿using DH;
using DH.Entity;

using Pek.Infrastructure;
using Pek.VirtualFileSystem;

using XCode;
using XCode.Membership;

namespace DG.Cube;

public class DHCubeStartup : IPekStartup {
    public void Configure(IApplicationBuilder application)
    {

    }

    /// <summary>
    /// 将区域路由写入数据库
    /// </summary>
    public void ConfigureArea()
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        if (!DHSetting.Current.IsInstalled)
        {
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
            if (!DG.Setting.Current.IsOnlyManager)
            {
                modelrole.IsSystem = true;
            }
            list.Add(modelrole);

            modelrole = Role.FindByID(4);
            modelrole.Name = "普通用户";
            modelrole.Remark = "普通业务人员，可以使用系统常规业务模块功能";
            if (!DG.Setting.Current.IsOnlyManager)
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
            if (!DG.Setting.Current.IsOnlyManager)
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

            DHSetting.Current.IsInstalled = true;
            DHSetting.Current.Save();
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
    public int StartupOrder => 99;

    /// <summary>
    /// 获取此启动配置实现的顺序。主要针对ConfigureMiddleware、UseRouting前执行的数据、UseAuthentication或者UseAuthorization后面 Endpoints前执行的数据
    /// </summary>
    public Int32 ConfigureOrder => 0;
}
