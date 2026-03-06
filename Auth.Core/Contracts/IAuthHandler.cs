using Hospital.Auth.UIViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Auth.Core.Contracts
{
    public interface IAuthHandler
    {
        Task<UserAuthResponse?> AuthenticateUserAsync(UserAuthRequest request);
    }
}
