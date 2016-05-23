using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using System.IdentityModel.Tokens;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;
using System;

namespace Lisa.Breakpoint.Api
{
    public static class TokenStartup
    {
        public static IApplicationBuilder UseJWTAuthenticationForBreakpoint(this IApplicationBuilder app, RsaSecurityKey key, TokenAuthOptions tokenOptions)
        {
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

                    if (error != null && error.Error is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(
                                new { authenticated = false, tokenExpired = true }));
                    }
                    else if (error != null && error.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(
                                new { success = false, error = error.Error.Message }));
                    }
                    else
                    {
                        await next();
                    }
                });
            });

            return app.UseJwtBearerAuthentication(options =>
            {
                options.TokenValidationParameters.IssuerSigningKey = key;
                options.TokenValidationParameters.ValidAudience = tokenOptions.Audience;
                options.TokenValidationParameters.ValidIssuer = tokenOptions.Issuer;
                options.TokenValidationParameters.ValidateSignature = true;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(0);
            });
        }
    }
}