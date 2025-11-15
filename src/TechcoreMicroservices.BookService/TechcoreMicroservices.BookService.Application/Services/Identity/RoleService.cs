using FluentResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Errors;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services.Identity;
using TechcoreMicroservices.BookService.Contracts.Requests.IdentityRoles;
using TechcoreMicroservices.BookService.Domain.Entities.Identity;

namespace TechcoreMicroservices.BookService.Application.Services.Identity;

public class RoleService : IRoleService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result> AssignRoleAsync(AssignRoleRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Fail(new NotFoundError("User with the provided email does not exist"));

        if (!(await _roleManager.RoleExistsAsync(request.Role)))
            return Result.Fail(new NotFoundError("The specified role does not exist"));

        var identityResult = await _userManager.AddToRoleAsync(user, request.Role);
        if(!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            var errorMessage = string.Join("; ", errors);

            return Result.Fail(new ConflictError(errorMessage));
        }

        return Result.Ok();
    }
}
