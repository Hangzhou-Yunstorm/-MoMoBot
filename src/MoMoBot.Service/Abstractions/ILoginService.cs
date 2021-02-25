using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MoMoBot.Service.Abstractions
{
    public interface ILoginService<TUser>
        where TUser : IdentityUser
    {
        Task<TUser> FindByUsername(string email);
        Task<bool> ValidateCredentials(TUser user, string password);
        Task SignIn(TUser user);
        Task SignOutAsync();
    }
}
