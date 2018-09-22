using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JwtAuthenticationCoreApi.Persistence.Repositories
{
    public interface IGenericRepository<TEntity> : IDisposable 
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> AllAsync();

        Task<IEnumerable<TEntity>> SearchForAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FindByIdAsync(int id);

        void Delete(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        void Update(TEntity entity);
    }
}