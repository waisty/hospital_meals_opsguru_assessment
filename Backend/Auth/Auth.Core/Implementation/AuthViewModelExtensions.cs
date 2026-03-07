using Hospital.Auth.Core.InternalModels;
using Hospital.Auth.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Auth.Core.Implementation
{
    internal static class AuthViewModelExtensions
    {
        public static UserAuthResponse ToUserAuthResponse(this User user, string tokenString)
        {
            return new UserAuthResponse()
            {
                AuthToken = tokenString,
                Admin = user.Admin,
                KitchenUser = user.Admin || user.KitchenUser,
                MealsAdmin = user.Admin || user.MealsAdmin,
                MealsUser = user.Admin || user.MealsUser,
                PatientAdmin = user.Admin || user.PatientAdmin
            };
        }
    
    }
}
