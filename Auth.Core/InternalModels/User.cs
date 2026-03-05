using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Auth.InternalModels
{
    internal class User
    {
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Name { get; set; } = "";
        public bool Admin { get; set; }
        public bool PatientAdmin { get; set; }
        public bool MealsAdmin { get; set; }
        public bool MealsUser { get; set; }
        public bool KitchenUser { get; set; }
    }

    
}
