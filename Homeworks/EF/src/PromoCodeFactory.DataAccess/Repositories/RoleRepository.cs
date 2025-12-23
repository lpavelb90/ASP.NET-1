using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using System;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class RoleRepository : EfRepository<Role, Guid>, IRoleRepository
    {
        public RoleRepository(ApplicationContext context) : base(context) { }
    }
}
