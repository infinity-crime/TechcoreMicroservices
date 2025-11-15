using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechcoreMicroservices.BookService.Contracts.Requests.User;

public record RegisterUserRequest(string Email, string Password, DateOnly DateOfBirth);
