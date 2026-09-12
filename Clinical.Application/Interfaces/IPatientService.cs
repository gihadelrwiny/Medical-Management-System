using Clinical.Application.Common;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.DTOs.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{
    public interface IPatientService
    {
        Task<PagedResult<PatientDto>> GetAllAsync(
            QueryParams query,
            CancellationToken ct);

     

        Task<PatientDto?> GetByIdAsync(
            int id,
            CancellationToken ct);

        Task UpdateAsync(
            int id,
            UpdatePatientDto dto,
            CancellationToken ct);

        Task<Result<bool>> DeleteAsync(
             int id,
           CancellationToken ct);
    }
}
