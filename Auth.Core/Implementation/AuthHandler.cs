using Auth.Core.Contracts;
using Auth.UIViewModels;
using Core.Auth.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Core.Implementation
{
    public class AuthHandler : IAuthHandler
    {
        private IAuthRepo authRepo;
        private IConfiguration configuration;
        public AuthHandler(IAuthRepo authRepo, IConfiguration configuration)
        {
            this.authRepo = authRepo;
            this.configuration = configuration;
        }
        public async Task<UserAuthResponse?> AuthenticateUserAsync(UserAuthRequest request)
        {
            var user = await this.authRepo.GetUserWithUsernameAndPasswordAsync(request.Username, request.Password);

            if (user == null)
            {
                return null;
            }
            else
            {
                var claimsList = new List<Claim>();
                claimsList.Add(new Claim("username", request.Username));
                if (user.Admin)
                {
                    claimsList.Add(new Claim("admin", user.Admin.ToString()));
                }
                if (user.MealsAdmin)
                {
                    claimsList.Add(new Claim("mealsAdmin", user.MealsAdmin.ToString()));
                }
                if (user.PatientAdmin)
                {
                    claimsList.Add(new Claim("patientAdmin", user.PatientAdmin.ToString()));
                }
                if (user.MealsUser)
                {
                    claimsList.Add(new Claim("mealsUser", user.MealsUser.ToString()));
                }
                if (user.KitchenUser)
                {
                    claimsList.Add(new Claim("kitchenUser", user.KitchenUser.ToString()));
                }

                var claims = claimsList.ToArray();

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new Exception("JWT configuration key not found")));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = creds,
                    Issuer = configuration["Jwt:Issuer"],
                    Audience = configuration["Jwt:Audience"]
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new UserAuthResponse()
                {
                    AuthToken = tokenString
                };
            }
        }
    }
}
