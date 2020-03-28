using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace SessionKeyManager
{
    public static class JwtExtensionsAndConstants
    {
        public const string JwtAuthenticationScheme = "JwtAuthScheme";
        public const string USERID_STRING = "userID";
#nullable enable
        public static string? GetUserIDFromJWTHeader(this HttpContext httpContext)
        {
            if (httpContext.User.Identity is ClaimsIdentity identity)
            {
                return identity.FindFirst(USERID_STRING)?.Value;
            }
            return null;
        }
        public static string? GetClaimValueFromHeader(this HttpContext httpContext, string claim)
        {
            if (httpContext.User.Identity is ClaimsIdentity identity)
            {
                return identity.FindFirst(claim)?.Value;
            }
            return null;
        }
#nullable disable

        /// <summary>
        /// Helper that adds jwt authentication to the AuthenticationBuilder with options validating issuer key and save token set to true
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <param name="key">The unhashed key used in signing the token</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwtAuthenticationWithKey(this AuthenticationBuilder builder, string key)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            return builder.AddJwtBearer(JwtAuthenticationScheme, x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Validate();
            });
        }
        public static AuthenticationBuilder AddJwtAuthenticationWithKeyAndIssuer(this AuthenticationBuilder builder, string key, string issuer)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return builder.AddJwtBearer(JwtAuthenticationScheme, x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidIssuer = issuer,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = true
                };
                x.Validate();
            });
        }
    }
}
