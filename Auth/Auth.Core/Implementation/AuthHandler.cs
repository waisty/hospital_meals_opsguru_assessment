using Hospital.Auth.Core.Contracts;
using Hospital.Auth.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hospital.Auth.Core.Implementation
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
                var claimsList = new List<Claim>
                {
                    new Claim(ClaimIds.usernameClaim, request.Username)
                };
                if (user.Admin)
                {
                    claimsList.Add(new Claim(ClaimIds.adminClaim, user.Admin.ToString()));
                }
                if (user.Admin || user.MealsAdmin)
                {
                    claimsList.Add(new Claim(ClaimIds.mealsAdminClaim, user.MealsAdmin.ToString()));
                }
                if (user.Admin || user.PatientAdmin)
                {
                    claimsList.Add(new Claim(ClaimIds.patientAdminClaim, user.PatientAdmin.ToString()));
                }
                if (user.Admin || user.MealsUser)
                {
                    claimsList.Add(new Claim(ClaimIds.mealsUserClaim, user.MealsUser.ToString()));
                }
                if (user.Admin || user.KitchenUser)
                {
                    claimsList.Add(new Claim(ClaimIds.kitchenUserClaim, user.KitchenUser.ToString()));
                }

                var claims = claimsList.ToArray();

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new Exception("JWT Key key not found")));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(6),
                    SigningCredentials = creds,
                    Issuer = configuration["Jwt:Issuer"] ?? throw new Exception("JWT Issuer not found"),
                    Audience = configuration["Jwt:Audience"] ?? throw new Exception("Jwt Audience not found")
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return user.ToUserAuthResponse(tokenString);
            }
        }
    }
}
