using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MoMoBot.Api
{
    public class MoMoBotIdentityErrorDescriber : IdentityErrorDescriber
    {

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code= "PasswordMismatch",
                Description="密码不匹配"
            };
        }

    }
}
