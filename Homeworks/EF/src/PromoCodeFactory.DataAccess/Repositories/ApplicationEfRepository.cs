using PromoCodeFactory.Core.Domain;
using System;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class ApplicationEfRepository<T, K>: EfRepository<T, K>
        where T : BaseEntity<K> where K : struct, IEquatable<K>
    {
        public ApplicationEfRepository(ApplicationContext context) : base(context) { }
    }
}
