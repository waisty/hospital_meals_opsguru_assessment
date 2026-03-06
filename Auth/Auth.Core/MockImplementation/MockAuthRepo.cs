using Hospital.Auth.Core.Contracts;
using Hospital.Auth.Core.InternalModels;
using System.Collections.Concurrent;

namespace Hospital.Auth.Core.MockImplementation
{
    /// <summary>
    /// In-memory mock implementation of IAuthRepo for testing.
    /// Add users and valid username/password pairs to control authentication results.
    /// </summary>
    public sealed class MockAuthRepo : IAuthRepo
    {
        private readonly ConcurrentDictionary<string, User> _usersByUsername = new();
        private readonly ConcurrentDictionary<(string Username, string Password), bool> _validLogins = new();

        /// <summary>
        /// Adds a user and marks a plain password as valid for login.
        /// </summary>
        public void AddUser(string username, string passwordForLogin, string name = "", bool admin = false, bool patientAdmin = false, bool mealsAdmin = false, bool mealsUser = false, bool kitchenUser = false)
        {
            var user = new User
            {
                Username = username,
                Name = name,
                PasswordHash = "",
                Admin = admin,
                PatientAdmin = patientAdmin,
                MealsAdmin = mealsAdmin,
                MealsUser = mealsUser,
                KitchenUser = kitchenUser
            };
            _usersByUsername[user.Username] = user;
            _validLogins[(username, passwordForLogin)] = true;
        }

        /// <summary>
        /// Marks a username/password pair as valid for GetUserWithUsernameAndPasswordAsync.
        /// The user must already be added via AddUser.
        /// </summary>
        public void AddValidLogin(string username, string password)
        {
            _validLogins[(username, password)] = true;
        }

        /// <summary>
        /// Clears all users and valid logins.
        /// </summary>
        public void Clear()
        {
            _usersByUsername.Clear();
            _validLogins.Clear();
        }

        Task<User?> IAuthRepo.GetUserAsync(string username)
        {
            _usersByUsername.TryGetValue(username, out var user);
            return Task.FromResult(user);
        }

        Task<User?> IAuthRepo.GetUserWithUsernameAndPasswordAsync(string username, string password)
        {
            if (!_validLogins.ContainsKey((username, password)))
                return Task.FromResult<User?>(null);
            _usersByUsername.TryGetValue(username, out var user);
            return Task.FromResult(user);
        }
    }
}
