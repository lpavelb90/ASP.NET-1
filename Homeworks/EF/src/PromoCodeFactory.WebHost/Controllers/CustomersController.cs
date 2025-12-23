using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController(ICustomerRepository customerRepository, IRepository<Preference, Guid> preferenceRepository)
        : ControllerBase
    {
        /// <summary>
        /// Получить всех клиентов
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync()
        {
            var result = await customerRepository.GetAll()
                .Select(x => new CustomerShortResponse
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    Id = x.Id,
                    LastName = x.LastName,
                })
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Получить клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var entity = await customerRepository.GetAll()
                .Include(x => x.Preferences)
                .Include(x => x.PromoCodes)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null)
                return NoContent();

            return Ok(new CustomerResponse
            {
                Id = entity.Id,
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Prefernces = entity.Preferences.Select(p => new PrefernceResponse
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList(),
                PromoCodes = entity.PromoCodes.Select(pc => new PromoCodeShortResponse
                {
                    Id = pc.Id,
                    Code = pc.Code,
                    PartnerName = pc.PartnerName,
                    ServiceInfo = pc.ServiceInfo,
                    BeginDate = pc.BeginDate.ToString("o"),
                    EndDate = pc.EndDate.ToString("o"),
                }).ToList()
            });

        }

        /// <summary>
        /// Создать клиента с его предпочтениями
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customer = new Customer
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            if (request.PreferenceIds.Any())
            {
                customer.Preferences = await preferenceRepository.GetAll()
                    .Where(p => request.PreferenceIds.Contains(p.Id))
                    .ToListAsync();

                if (customer.Preferences.Count != request.PreferenceIds.Count)
                    return BadRequest("Одно или несколько предпочтений не найдены");
            }

            await customerRepository.CreateAsync(customer);

            return Created();
        }

        /// <summary>
        /// Обновить данные клиента вместе с его предпочтениями
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await customerRepository.GetAll()
                .Include(x => x.Preferences)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound();

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;

            if (request.PreferenceIds.Any())
            {
                var newPreferences = await preferenceRepository.GetAll()
                    .Where(p => request.PreferenceIds.Contains(p.Id))
                    .ToListAsync();

                if (newPreferences.Count != request.PreferenceIds.Count)
                    return BadRequest("Одно или несколько предпочтений не найдены");

                customer.Preferences = newPreferences;
            }

            await customerRepository.UpdateAsync(customer);

            return Ok();
        }

        /// <summary>
        /// Удалить клиента вместе с выданными ему промокодами
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            // и каскадное удаление промокодов на уровне БД
            await customerRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}