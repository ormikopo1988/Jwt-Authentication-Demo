using System;
using System.Threading.Tasks;
using JwtAuthenticationCoreApi.Persistence.Repositories;

namespace JwtAuthenticationCoreApi.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IJwtAuthUserRepository JwtAuthUsersRepository { get; }

        Task CompleteAsync();
    }
}