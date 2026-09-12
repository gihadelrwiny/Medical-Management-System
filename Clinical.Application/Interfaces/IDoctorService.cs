using Clinical.Application.Common;
using Clinical.Application.DTOs.Doctor;
using Clinical.Application.DTOs.Pagination;


namespace Clinical.Application.Interfaces;

public interface IDoctorService
{
    Task<PagedResult<DoctorDto>> GetAllAsync(
    QueryParams query,
    CancellationToken ct);
    

    Task<DoctorDto?> GetByIdAsync(
        int id,
        CancellationToken ct);

    Task<Result<DoctorDto>> CreateAsync(
  CreateDoctorRequest request,
  CancellationToken ct);

    Task UpdateAsync(
        int id,
        UpdateDoctorDto dto,
        CancellationToken ct);

    Task DeleteAsync(
        int id,
        CancellationToken ct);
}