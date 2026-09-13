using Clinical.Application.DTOs.DoctorSchedule;
using Clinical.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{
    public interface IDoctorScheduleService
    {
        Task<PagedResult<DoctorScheduleDto>> GetAllAsync(
            QueryParams query,
            CancellationToken cancellationToken = default);

        Task<DoctorScheduleDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<DoctorScheduleDto> CreateAsync(
            CreateDoctorScheduleDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            int id,
            UpdateDoctorScheduleDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
