using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Protos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Services
{
    public class GivingPromoCodeToCustomerService : GivingPromoCodeToCustomer.GivingPromoCodeToCustomerBase
    {
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<Customer> _customersRepository;
        private readonly IRepository<PromoCode> _promoCodesRepository;

        public GivingPromoCodeToCustomerService(IRepository<Preference> preferencesRepository,
            IRepository<Customer> customersRepository,
            IRepository<PromoCode> promoCodesRepository)
        {
            _preferencesRepository = preferencesRepository;
            _customersRepository = customersRepository;
            _promoCodesRepository = promoCodesRepository;
        }

        public override async Task<Empty> GivePromoCodeToCustomer(GivePromoCodeToCustomerRequest request, ServerCallContext context)
        {
            //Получаем предпочтение по имени
            var preference = await _preferencesRepository.GetByIdAsync(Guid.Parse(request.PreferenceId));

            if (preference == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Preference was not found"));
            }

            //  Получаем клиентов с этим предпочтением:
            var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

            PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preference, customers);

            await _promoCodesRepository.AddAsync(promoCode);

            return new Empty();
        }
    }
}
