using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SessionKeyManager
{
    public static class ExtensionMethods
    {

#nullable enable
        public static string? GetUserIDFromJWTHeader(this HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return identity.FindFirst(JWTSessionKeyManager.USERID_STRING)?.Value;
            }
            return null;
        }
        public static string? GetClaimValueFromHeader(this HttpContext httpContext, string claim)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return identity.FindFirst(claim)?.Value;
            }
            return null;
        }

        /// <summary>
        /// Helper that adds jwt authentication to the AuthenticationBuilder with options validating issuer key and save token set to true
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <param name="key">The unhashed key used in signing the token</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwtAuthentication(this AuthenticationBuilder builder,string key)
        {
            var signingKey = new SymmetricSecurityKey(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key)));
            return builder.AddJwtBearer(JwtAuthenticationHelper.JwtAuthenticationScheme, x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
