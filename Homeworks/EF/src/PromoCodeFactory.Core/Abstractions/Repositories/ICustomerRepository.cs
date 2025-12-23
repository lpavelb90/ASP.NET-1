using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface ICustomerRepository: IRepository<Customer, Guid>
    {
    }
}
