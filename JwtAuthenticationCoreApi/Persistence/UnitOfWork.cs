using System.Threading.Tasks;
using JwtAuthenticationCoreApi.Core;
using JwtAuthenticationCoreApi.Persistence.Repositories;

namespace JwtAuthenticationCoreApi.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IJwtAuthUserRepository JwtAuthUsersRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context, IJwtAuthUserRepository jwtAuthUsersRepository)
        {
            _context = context;
            JwtAuthUsersRepository = jwtAuthUsersRepository;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}