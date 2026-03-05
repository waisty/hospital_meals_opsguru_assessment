using Auth.UIViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Core.Contracts
{
    public interface IAuthHandler
    {
        Task<UserAuthResponse?> AuthenticateUserAsync(UserAuthRequest request);
    }
}
