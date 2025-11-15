using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Authorization.Requirements;

namespace TechcoreMicroservices.BookService.Application.Authorization.Handlers;

public class AgeHandler : AuthorizationHandler<AgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgeRequirement requirement)
    {
        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);
        if (dateOfBirthClaim is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Birthday claim not found!"));
            return Task.CompletedTask;
        }

        if (!DateTime.TryParse(dateOfBirthClaim.Value, out DateTime dateOfBirth))
        {
            context.Fail(new AuthorizationFailureReason(this, "Birthday claim is not valid date!"));
            return Task.CompletedTask;
        }

        int userAge = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-userAge))
            userAge--;

        if (userAge >= requirement.MinimumAge)
            context.Succeed(requirement);
        else
            context.Fail(new AuthorizationFailureReason(this, "User does not meet the minimum age requirement!"));

        return Task.CompletedTask;
    }
}
