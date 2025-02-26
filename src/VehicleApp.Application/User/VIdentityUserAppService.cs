﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace VehicleApp.Application.User;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IIdentityUserAppService), typeof(IdentityUserAppService), typeof(VIdentityUserAppService))]
public class VIdentityUserAppService(IdentityUserManager userManager,
                                     IIdentityUserRepository userRepository,
                                     IIdentityRoleRepository roleRepository,
                                     IOptions<IdentityOptions> identityOptions,
                                     IPermissionChecker permissionChecker) :
    IdentityUserAppService(userManager, userRepository, roleRepository, identityOptions, permissionChecker)
{
    public override async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
    {
        Guid? organizationUnitId = input.GetProperty<Guid?>("OrganizationUnitId") ?? null;
        bool? isActive = input.GetProperty<bool?>("IsActive");

        long count = await UserRepository.GetCountAsync(input.Filter);
        var list = await UserRepository.GetListAsync(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.Filter,
            organizationUnitId: organizationUnitId,
            notActive: isActive);

        return new PagedResultDto<IdentityUserDto>(
            count,
            ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(list)
        );
    }

    public override async Task DeleteAsync(Guid id)
    {
        if (CurrentUser.Id == id)
        {
            throw new BusinessException(code: IdentityErrorCodes.UserSelfDeletion);
        }

        var user = await UserManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return;
        }

        if (user.UserName.ToLower() == "admin")
        {
            throw new BusinessException(code: "400", message: "不能删除超级管理员");
        }

        (await UserManager.DeleteAsync(user)).CheckErrors();
    }

    public override async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
    {
        return await base.CreateAsync(input);
    }
}
