using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Contracts.Requests.IdentityRoles;

namespace TechcoreMicroservices.BookService.Application.Common.Interfaces.Services.Identity;

public interface IRoleService
{
    Task<Result> AssignRoleAsync(AssignRoleRequest request);
}
