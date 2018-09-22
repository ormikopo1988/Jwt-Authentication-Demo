using Microsoft.EntityFrameworkCore;
using JwtAuthenticationCoreApi.Core.Models.Entities;

namespace JwtAuthenticationCoreApi.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<JwtAuthUser> JwtAuthUsers { get; set; }
    }
}