using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T, K>
        where T : BaseEntity<K> where K : struct, IEquatable<K>
    {
        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(K id);

        Task DeleteAsync(K id);

        Task<K> CreateAsync(T entity);

        Task UpdateAsync(T entity);
    }
}