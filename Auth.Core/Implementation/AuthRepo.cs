using Core.Auth.Contracts;
using Core.Auth.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
            // Verify password using pgcrypto: crypt(input, stored_hash) equals stored_hash when password is correct.
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

    internal class AuthDBContext : DbContext
    {
        private readonly IConfiguration configuration;
        public AuthDBContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(this.configuration["ConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        //public static string Crypt(string password, string salt) => throw new NotSupportedException();
        //public static string GenSalt(string type) => throw new NotSupportedException();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<User>(User.Configure);

            //modelBuilder.HasDbFunction(typeof(AuthDBContext).GetMethod(nameof(Crypt), [typeof(string), typeof(string)]) ?? throw new Exception($"{nameof(Crypt)} function not found in ${nameof(AuthDBContext)}"))
            //    .HasName("crypt").HasSchema(null);

            //modelBuilder.HasDbFunction(typeof(AuthDBContext).GetMethod(nameof(GenSalt), [typeof(string)]) ?? throw new Exception($"{nameof(GenSalt)} function not found in ${nameof(AuthDBContext)}"))
            //    .HasName("gen_salt").HasSchema(null);
        }
    }

//    await using var ctx = new BlogContext();
//    await ctx.Database.EnsureDeletedAsync();
//    await ctx.Database.EnsureCreatedAsync();

//    // Insert a Blog
//    ctx.Blogs.Add(new () { Name = "FooBlog" });
//await ctx.SaveChangesAsync();

//    // Query all blogs who's name starts with F
//    var fBlogs = await ctx.Blogs.Where(b => b.Name.StartsWith("F")).ToListAsync();

//    public class BlogContext : DbContext
//    {
//        public DbSet<Blog> Blogs { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//            => optionsBuilder.UseNpgsql(@"Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase");
//    }

//    public class Blog
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//    }
}
