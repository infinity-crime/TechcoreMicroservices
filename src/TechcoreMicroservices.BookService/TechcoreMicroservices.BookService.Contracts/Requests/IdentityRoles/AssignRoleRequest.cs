using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Requests.IdentityRoles;

public record AssignRoleRequest(string Email, string Role);
