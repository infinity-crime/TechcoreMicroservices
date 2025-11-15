using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Domain.Entities.Identity;

public class User : IdentityUser
{
    public DateOnly DateOfBirth { get; set; }
}
