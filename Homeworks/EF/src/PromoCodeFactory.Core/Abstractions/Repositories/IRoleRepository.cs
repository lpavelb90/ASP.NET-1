using PromoCodeFactory.Core.Domain.Administration;
using System;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRoleRepository: IRepository<Role, Guid>
    {
    }
}
