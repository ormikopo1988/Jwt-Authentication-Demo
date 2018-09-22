using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JwtAuthenticationCoreApi.Core.Models.Entities;

namespace JwtAuthenticationCoreApi.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<JwtAuthUser> JwtAuthUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JwtAuthUser>().ToTable("JwtAuthUsers");
            
            base.OnModelCreating(modelBuilder);
        }
    }
}