﻿using Microsoft.Extensions.DependencyInjection;
using VehicleApp.Application.Contracts;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.VirtualFileSystem;

namespace VehicleApp.HttpApi.Client;

[DependsOn(
    typeof(VehicleAppApplicationContractsModule),
    typeof(AbpPermissionManagementHttpApiClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule),
    typeof(AbpAccountHttpApiClientModule),
    typeof(AbpIdentityHttpApiClientModule),
    typeof(AbpTenantManagementHttpApiClientModule),
    typeof(AbpSettingManagementHttpApiClientModule)
)]
public class VehicleAppHttpApiClientModule : AbpModule
{
    public const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(VehicleAppApplicationContractsModule).Assembly,
            RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<VehicleAppHttpApiClientModule>();
        });
    }
}
