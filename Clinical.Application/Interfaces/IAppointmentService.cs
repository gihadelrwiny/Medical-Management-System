using Clinical.Application.DTOs.Appoinment;
using Clinical.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<PagedResult<AppointmentDto>> GetAllAsync(QueryParams query, CancellationToken cancellationToken = default);
        Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int id, UpdateAppointmentDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
     
    }
}
