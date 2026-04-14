using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Repositories
{
    public interface IPreferenceRepository
    {
        Task<Preference> GetByIdAsync(Guid id);
        Task<List<Preference>> GetAllAsync();
    }
}
