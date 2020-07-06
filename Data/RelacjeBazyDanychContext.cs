using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JWTAutentication.Models;

namespace JWTAutentication.Data
{
    public class RelacjeBazyDanychContext : IdentityDbContext
    {
        public RelacjeBazyDanychContext(DbContextOptions<RelacjeBazyDanychContext> options)
            : base(options)
        {
        }

        public DbSet<Owner> Owners { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}