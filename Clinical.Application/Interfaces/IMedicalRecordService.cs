using Clinical.Application.Common;
using Clinical.Application.DTOs.MedicalRecord;
using Clinical.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{


    public interface IMedicalRecordService
    {
        Task<PagedResult<MedicalRecordDto>> GetAllAsync(QueryParams query, CancellationToken ct);
        Task<MedicalRecordDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<Result<MedicalRecordDto>> CreateAsync(CreateMedicalRecordRequest request, CancellationToken ct);
        Task UpdateAsync(int id, UpdateMedicalRecordDto dto, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
