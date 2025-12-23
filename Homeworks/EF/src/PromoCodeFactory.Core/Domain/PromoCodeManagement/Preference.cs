using System;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity<Guid>
    {
        [MaxLength(128)]
        public string Name { get; set; }

        public PromoCode PromoCode { get; set; }
    }
}