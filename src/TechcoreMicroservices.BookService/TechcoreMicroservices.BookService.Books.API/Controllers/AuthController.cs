using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services.Identity;
using TechcoreMicroservices.BookService.Books.API.Controllers.Common;
using TechcoreMicroservices.BookService.Contracts.Requests.User;
using TechcoreMicroservices.BookService.Contracts.Responses.User;

namespace TechcoreMicroservices.BookService.Books.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var result = await _userService.RegisterAsync(request);

        return HandleResult<IdentityResult>(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        var result = await _userService.LoginAsync(request);

        return HandleResult<UserResponse>(result);
    }
}
