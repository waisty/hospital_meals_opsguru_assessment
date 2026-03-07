using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Auth.ViewModels
{
    public class UserAuthResponse
    {
        public string AuthToken { get; set; } = "";
        public bool Admin { get; set; }
        public bool PatientAdmin { get; set; }
        public bool MealsAdmin { get; set; }
        public bool MealsUser { get; set; }
        public bool KitchenUser { get; set; }
    }
}
