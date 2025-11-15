using FluentResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Requests.User;
using TechcoreMicroservices.BookService.Contracts.Responses.User;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Services.Identity;

public interface IUserService
{
    Task<Result<IdentityResult>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default);
}
