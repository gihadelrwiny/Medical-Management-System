using Clinical.Application.DTOs.Department;
using Clinical.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{
    public interface IDepartmentService
    {
         Task<PagedResult<DepartmentDto>> GetAllAsync(QueryParams query,CancellationToken cancellationToken = default);
        Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
