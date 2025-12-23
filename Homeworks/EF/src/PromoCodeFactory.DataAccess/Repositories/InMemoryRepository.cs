using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T, K>
        : IRepository<T, K> where T : BaseEntity<K> where K : struct, IEquatable<K>
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public IQueryable<T> GetAll() => Data.AsQueryable();

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(K id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id.Equals(id)));
        }

        public Task DeleteAsync(K id)
        {
            throw new NotImplementedException();
        }

        public Task<K> CreateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}