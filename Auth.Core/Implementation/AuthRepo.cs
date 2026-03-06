using Core.Auth.Contracts;
using Core.Auth.InternalModels;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Implementation
{
    internal class AuthRepo : IAuthRepo
    {
        private readonly AuthDBContext context;

        public AuthRepo(AuthDBContext authContext)
        {
            this.context = authContext;
        }

        public async Task<User?> GetUserAsync(string username)
        {
            var user = await (from u in context.Users where u.Username == username select u).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User?> GetUserWithUsernameAndPasswordAsync(string username, string password)
        {
            var user = await context.Users
                .FromSqlRaw(
                    "SELECT * FROM dbo.users WHERE username = {0} AND password_hash = crypt({1}, password_hash)",
                    username,
                    password)
                .AsNoTracking()
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            return user;
        }
    }
}
