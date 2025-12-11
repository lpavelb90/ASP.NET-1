using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = [.. data];
        }

        public Task<List<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(T entity)
        {
            Data.Add(entity);

            return Task.CompletedTask;
        }

        public async Task UpdateAsync(T entity)
        {
            var existingEntity = await GetByIdAsync(entity.Id);
            if (entity != null)
            {
                var index = Data.IndexOf(entity);
                Data[index] = entity;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);

            if (entity != null)
            {
                Data.Remove(entity);

                return true;
            }

            return false;
        }
    }
}