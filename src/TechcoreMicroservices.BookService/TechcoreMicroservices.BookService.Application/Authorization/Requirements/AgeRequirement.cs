using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Application.Authorization.Requirements;

public class AgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public AgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}
