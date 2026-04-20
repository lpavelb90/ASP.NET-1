using Pcf.Administration.Core.Domain.Administration;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Services
{
    public interface IEmployeeService
    {
        Task<Employee> UpdateAppliedPromocodesAsync(Guid employeeId);
    }
}
