using MassTransit;
using Pcf.Integration.Messages;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using System;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class AdministrationGateway
        : IAdministrationGateway
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public AdministrationGateway(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId)
        {
            await _publishEndpoint.Publish(new PromoCodeFromPartnerIssuedMessage { PartnerManagerId = partnerManagerId });
        }
    }
}