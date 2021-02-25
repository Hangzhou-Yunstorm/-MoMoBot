using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MoMoBot.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static T IdentityUser<T>(this HttpContext context, TokenValidationParameters validationParameters) where T : IdentityUser, new()
        {
            var token = GetToken(context);
            if (string.IsNullOrEmpty(token) == false)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var _);
                    var claims = claimsPrincipal.Claims;
                    if (claims?.Count() > 0)
                    {
                        var user = new T();
                        user.Id = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
                        user.UserName = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value;
                        user.Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                        return user;
                    }
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }

        private static string GetToken(HttpContext context)
        {
            var token = context.Request.Headers.FirstOrDefault(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase)).Value.ToString();

            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Query["access_token"];
            }
            else
            {
                if(token.StartsWith("Bearer "))
                {
                    token = token.Replace("Bearer ", "");
                }
            }
            return token;
        }
    }
}
