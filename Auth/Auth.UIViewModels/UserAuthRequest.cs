using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Auth.ViewModels
{
    public class UserAuthRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
