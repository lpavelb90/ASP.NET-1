using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Integration.Protos;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerGateway
        : IGivingPromoCodeToCustomerGateway
    {
        private readonly GivingPromoCodeToCustomer.GivingPromoCodeToCustomerClient _givingPromoCodeToCustomerClient;

        public GivingPromoCodeToCustomerGateway(GivingPromoCodeToCustomer.GivingPromoCodeToCustomerClient givingPromoCodeToCustomerClient)
        {
            _givingPromoCodeToCustomerClient = givingPromoCodeToCustomerClient;
        }

        public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        {
            var request = new GivePromoCodeToCustomerRequest
            {
                PartnerId = promoCode.Partner.Id.ToString(),
                BeginDate = promoCode.BeginDate.ToShortDateString(),
                EndDate = promoCode.EndDate.ToShortDateString(),
                PreferenceId = promoCode.PreferenceId.ToString(),
                PromoCode = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerManagerId = promoCode.PartnerManagerId.ToString(),
            };

            await _givingPromoCodeToCustomerClient.GivePromoCodeToCustomerAsync(request);
        }
    }
}