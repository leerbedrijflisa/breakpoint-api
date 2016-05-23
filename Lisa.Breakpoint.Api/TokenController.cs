using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Lisa.Breakpoint.Api
{
    [Route("/token/")]
    public class TokenController : Controller
    {
        private readonly TokenAuthOptions tokenOptions;

        public TokenController(TokenAuthOptions tokenOptions)
        {
            this.tokenOptions = tokenOptions;
        }
        [HttpGet]
        [Authorize("Bearer")]
        public async Task<ActionResult> Get()
        {
            bool authenticated = false;
            string username = null;
            int entityId = -1;
            string token = null;
            DateTime? tokenExpires = default(DateTime?);

            var currentUser = HttpContext.User;
            if (currentUser != null)
            {
                authenticated = currentUser.Identity.IsAuthenticated;
                if (authenticated)
                {
                    username = currentUser.Identity.Name;
                    foreach (Claim c in currentUser.Claims) if (c.Type == "EntityID") entityId = Convert.ToInt32(c.Value);
                    tokenExpires = DateTime.UtcNow.AddHours(2);
                    token = GetToken(currentUser.Identity.Name, tokenExpires);
                }
            }
            var tokenResponse = new TokenResponse
            {
                User = username,
                Token = token,
                TokenExpires = tokenExpires
            };

            return new HttpOkObjectResult(tokenResponse);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AuthRequest req)
        {
            if (!ModelState.IsValid || req == null)
            {
                return new HttpUnauthorizedResult();
            }

            DateTime? expires = DateTime.UtcNow.AddHours(2);
            var token = GetToken(req.username, expires);

            var tokenResponse = new TokenResponse
            {
                User = req.username,
                Token = token,
                TokenExpires = expires
            };

            return new HttpOkObjectResult(tokenResponse);
        }

        private string GetToken(string username, DateTime? expires)
        {
            var handler = new JwtSecurityTokenHandler();

            var user = username;

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, username, ClaimValueTypes.String));

            var identity = new ClaimsIdentity(new GenericIdentity(username, "TokenAuth"), claims);

            var securityToken = handler.CreateToken(
                audience: tokenOptions.Audience,
                issuer: tokenOptions.Issuer,
                signingCredentials: tokenOptions.SigningCredentials,
                subject: identity,
                expires: expires
                );
            return handler.WriteToken(securityToken);
        }
    }
}