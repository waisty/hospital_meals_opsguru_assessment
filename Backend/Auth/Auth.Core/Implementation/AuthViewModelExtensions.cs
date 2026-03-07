using Hospital.Auth.Core.InternalModels;
using Hospital.Auth.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Auth.Core.Implementation
{
    internal static class AuthViewModelExtensions
    {
        public static UserAuthResponse ToUserAuthResponse(this User user, string tokenString, bool admin, bool kitchenUser, bool mealsAdmin, bool mealsUser, bool patientAdmin)
        {
            return new UserAuthResponse()
            {
                AuthToken = tokenString,
                Admin = admin,
                KitchenUser = kitchenUser,
                MealsAdmin = mealsAdmin,
                MealsUser = mealsUser,
                PatientAdmin = patientAdmin
            };
        }
    
    }
}
