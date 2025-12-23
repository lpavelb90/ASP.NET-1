using System;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Role
        : BaseEntity<Guid>
    {
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public Employee Employee { get; set; }
    }
}