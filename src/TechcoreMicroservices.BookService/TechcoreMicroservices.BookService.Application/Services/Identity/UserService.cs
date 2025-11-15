using FluentResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Errors;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Authentication;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services.Identity;
using TechcoreMicroservices.BookService.Contracts.Requests.User;
using TechcoreMicroservices.BookService.Contracts.Responses.User;
using TechcoreMicroservices.BookService.Domain.Entities.Identity;

namespace TechcoreMicroservices.BookService.Application.Services.Identity;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    private readonly IJwtTokenGenerator _tokenGenerator;
    public UserService(UserManager<User> userManager, 
        SignInManager<User> signInManager,
        IJwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<UserResponse>> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Fail(new NotFoundError($"User with email {request.Email} was not found."));

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!signInResult.Succeeded)
            return Result.Fail(new ValidationError("The provided credentials are invalid"));

        var token = _tokenGenerator.GenerateToken(user);

        return Result.Ok(new UserResponse(user.Email!, user.DateOfBirth, token));
    }

    public async Task<Result<IdentityResult>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var newUser = new User
        {
            UserName = request.Email,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth
        };

        var identityResult = await _userManager.CreateAsync(newUser, request.Password);
        if(!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            var errorMessage = string.Join(';', errors);

            return Result.Fail(new ValidationError(errorMessage));
        }

        return Result.Ok(identityResult);
    }
}
