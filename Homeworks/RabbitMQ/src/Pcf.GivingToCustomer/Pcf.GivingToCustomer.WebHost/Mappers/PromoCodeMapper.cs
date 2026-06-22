using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;
using Pcf.GivingToCustomer.WebHost.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Mappers
{
    public class PromoCodeMapper
    {
        public static PromoCode MapFromModel(GivePromoCodeToCustomerRequest request, Preference preference, IEnumerable<Customer> customers)
        {

            var promocode = new PromoCode();
            if (Guid.TryParse(request.PromoCodeId, out Guid guid))
            {
                promocode.Id = guid;
            }

            promocode.PartnerId = Guid.Parse(request.PartnerId);
            promocode.Code = request.PromoCode;
            promocode.ServiceInfo = request.ServiceInfo;

            promocode.BeginDate = DateTime.Parse(request.BeginDate);
            promocode.EndDate = DateTime.Parse(request.EndDate);

            promocode.Preference = preference;
            promocode.PreferenceId = preference.Id;

            promocode.Customers = new List<PromoCodeCustomer>();

            foreach (var item in customers)
            {
                promocode.Customers.Add(new PromoCodeCustomer()
                {

                    CustomerId = item.Id,
                    Customer = item,
                    PromoCodeId = promocode.Id,
                    PromoCode = promocode
                });
            }

            return promocode;
        }

        public static PromoCode MapFromModel(GivePromoCodeRequest request, Preference preference, IEnumerable<Customer> customers)
        {

            var promocode = new PromoCode();
            promocode.Id = request.PromoCodeId;

            promocode.PartnerId = request.PartnerId;
            promocode.Code = request.PromoCode;
            promocode.ServiceInfo = request.ServiceInfo;

            promocode.BeginDate = DateTime.Parse(request.BeginDate);
            promocode.EndDate = DateTime.Parse(request.EndDate);

            promocode.Preference = preference;
            promocode.PreferenceId = preference.Id;

            promocode.Customers = new List<PromoCodeCustomer>();

            foreach (var item in customers)
            {
                promocode.Customers.Add(new PromoCodeCustomer()
                {

                    CustomerId = item.Id,
                    Customer = item,
                    PromoCodeId = promocode.Id,
                    PromoCode = promocode
                });
            };

            return promocode;
        }
    }
}
