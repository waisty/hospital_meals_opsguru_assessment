using Auth.UIViewModels;
using Core.Auth.InternalModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Auth.Contracts
{
    public interface IAuthRepo
    {
        internal Task<User?> GetUserAsync(string username);
        internal Task<User?> GetUserWithUsernameAndPasswordAsync(string username, string password);
    }
}
