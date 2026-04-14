using Microsoft.Extensions.Caching.Distributed;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.DataAccess.Repositories
{
    public class PreferenceRepository: IPreferenceRepository
    {
        private readonly IRepository<Preference> _preferenceRepository;
        private readonly IDistributedCache _cache;

        public PreferenceRepository(IRepository<Preference> preferenceRepository, IDistributedCache cache)
        {
            _preferenceRepository = preferenceRepository;
            _cache = cache;
        }

        private async Task<List<Preference>> GetOrAddFromCacheAsync()
        {
            var key = nameof(PreferenceRepository);
            var value = _cache.GetString(key);
            var preferences = string.IsNullOrEmpty(value) 
                ? [] 
                : JsonSerializer.Deserialize<List<Preference>>(value);

            if (preferences == null || !preferences.Any())
            {
                preferences = (await _preferenceRepository.GetAllAsync()).ToList();

                if (preferences.Any())
                {
                    value = JsonSerializer.Serialize(preferences);
                    _cache.SetStringAsync(key, value);
                }
            }

            return  preferences;
        }

        public async Task<List<Preference>> GetAllAsync() => await GetOrAddFromCacheAsync();

        public async Task<Preference> GetByIdAsync(Guid id)
        {
            var preferences = await GetOrAddFromCacheAsync();

            return preferences?.FirstOrDefault(x => x.Id == id);
        }
    }
}
