using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace txtOnly.Infrastructure.Identity.Interfaces;

public interface IAuthenticate
{
    Task<bool> Authenticate(string email, string password);
    Task<(bool Success, string? ErrorMessage)> RegisterUser(string email, string password);
    Task Logout();
}
