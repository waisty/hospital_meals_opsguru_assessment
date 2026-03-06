using Hospital.Auth.UIViewModels;
using Hospital.Auth.Core.InternalModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Auth.Core.Contracts
{
    public interface IAuthRepo
    {
        internal Task<User?> GetUserAsync(string username);
        internal Task<User?> GetUserWithUsernameAndPasswordAsync(string username, string password);
    }
}
