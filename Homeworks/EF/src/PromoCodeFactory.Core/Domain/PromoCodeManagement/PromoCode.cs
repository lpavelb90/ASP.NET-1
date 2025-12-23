using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PromoCode
        : BaseEntity<Guid>
    {
        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(512)]
        public string ServiceInfo { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        [MaxLength(128)]
        public string PartnerName { get; set; }

        public Employee PartnerManager { get; set; }

        public Guid PreferenceId { get; set; }

        public Preference Preference { get; set; }
    }
}