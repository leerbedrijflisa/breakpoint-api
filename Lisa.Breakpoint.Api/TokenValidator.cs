using System;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Lisa.Breakpoint.Api
{
    public static class TokenValidator
    {
        private static TokenValidationParameters tokenValidation = new TokenValidationParameters
        {
            IssuerSigningKey = RSAKeyUtils.GetRSAKey(),
            ValidAudience = "Breakpoint",
            ValidIssuer = "Breakpoint",
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true
        };
        private static JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        private static SecurityToken validatedToken = null;
        public static string ValidateAndReadSentToken(string token)
        {
            try
            {
                handler.ValidateToken(token, tokenValidation, out validatedToken);
            }catch (Exception)
            {
                return null;
            }
            
            if (handler.CanReadToken(token) == false)
            {
                return null;
            }
            else
            {
                dynamic readToken = handler.ReadToken(token);
                var username = readToken.Claims[3].Value;
                return username;
            }
        }
    }
}