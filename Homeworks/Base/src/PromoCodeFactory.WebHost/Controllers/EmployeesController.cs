using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync(EmployeeRequest request)
        {
            var guid = Guid.NewGuid();

            await _employeeRepository.AddAsync(new Employee()
            {
                Id = guid,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AppliedPromocodesCount = request.AppliedPromocodesCount,
                Roles = request.Roles?.Select(x => new Role()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList() ?? new List<Role>(),
                Email = request.Email,
            });

            return CreatedAtAction(nameof(GetEmployeeByIdAsync), "Employees", new { id = guid }, null);
        }

        /// <summary>
        /// Обновить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid id, EmployeeRequest request)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);

            if (existingEmployee == null)
                return NotFound();

            existingEmployee.FirstName = request.FirstName;
            existingEmployee.LastName = request.LastName;
            existingEmployee.Email = request.Email;
            existingEmployee.AppliedPromocodesCount = request.AppliedPromocodesCount;
            existingEmployee.Roles = request.Roles?.Select(x => new Role()
            {
                Name = x.Name,
                Description = x.Description
            }).ToList() ?? new List<Role>();

            await _employeeRepository.UpdateAsync(existingEmployee);
            return Ok();
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <param name="id">Id сотрудника</param>
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var found = await _employeeRepository.DeleteAsync(id);

            if (!found)
                return NotFound();

            return NoContent();
        }
    }
}