using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens;

namespace Lisa.Breakpoint.Api
{
    public class TokenAuthOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }

    public class AuthRequest
    {
        [Required]
        public string username { get; set; }
    }

    public class TokenResponse
    {
        public string User { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpires { get; set; }
    }
}