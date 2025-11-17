using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services.Identity;
using TechcoreMicroservices.BookService.Books.API.Controllers.Common;
using TechcoreMicroservices.BookService.Contracts.Requests.IdentityRoles;

namespace TechcoreMicroservices.BookService.Books.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : BaseController
{
    private readonly IRoleService _roleService;

    public AdminController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var result = await _roleService.AssignRoleAsync(request);

        return HandleResult(result);
    }
}
