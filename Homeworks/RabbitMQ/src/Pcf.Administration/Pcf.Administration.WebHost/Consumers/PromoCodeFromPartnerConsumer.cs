using MassTransit;
using Pcf.Administration.Core.Services;
using Pcf.Integration.Messages;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PromoCodeFromPartnerConsumer : IConsumer<PromoCodeFromPartnerIssuedMessage>
    {
        private readonly IEmployeeService _employeeService;
        public PromoCodeFromPartnerConsumer(IEmployeeService employeeService) 
        { 
            _employeeService = employeeService;
        }

        public async Task Consume(ConsumeContext<PromoCodeFromPartnerIssuedMessage> context)
        {
            await _employeeService.UpdateAppliedPromocodesAsync(context.Message.PartnerManagerId);
        }
    }
}
