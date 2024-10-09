using GiveandTake_Repo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business.Utils
{
    public class JwtUtils
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtUtils(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GenerateJwtToken(Account account)
        {
            // Load configuration from appsettings.json
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var key = config["Jwt:Key"];
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];

            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>()
                            {
                                new Claim("AccountId", account.AccountId.ToString()),
                                new Claim("FullName", account.FullName),
                                new Claim("Email", account.Email),
                                new Claim("Password", account.Password),
                                new Claim(ClaimTypes.Role, account.RoleId.ToString(), ClaimValueTypes.Integer32),
                                new Claim("IsPremium", (bool)account.IsPremium ? "true" : "false")
                            };

            // Add expiredTime of token
            var expires = DateTime.Now.AddMinutes(30);

            // Create token
            var token = new JwtSecurityToken(issuer, audience, claims, notBefore: DateTime.Now, expires, credentials);
            return jwtHandler.WriteToken(token);
        }
    }
}
