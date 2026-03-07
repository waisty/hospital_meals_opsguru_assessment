using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Auth.Core.Contracts
{
    public class ClaimIds
    {
        public const string usernameClaim = "username";
        public const string adminClaim = "admin";
        public const string mealsAdminClaim = "mealsAdmin";
        public const string patientAdminClaim = "patientAdmin";
        public const string mealsUserClaim = "mealsUser";
        public const string kitchenUserClaim = "kitchenUser";
    }
}
