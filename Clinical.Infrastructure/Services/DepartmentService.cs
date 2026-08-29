using AutoMapper;
using Clinical.Application.DTOs.Department;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using System.Linq.Expressions;

namespace Clinical.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DepartmentService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DepartmentDto>> GetAllAsync(
        QueryParams query,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<Department, bool>>? filter = null;

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            filter = d =>
                d.Name.Contains(search) ||
                d.Description.Contains(search);
        }

        // Sorting
        Func<IQueryable<Department>, IQueryable<Department>> orderBy =
            query.SortBy.ToLower() switch
            {
                "name" => query.SortDir.ToLower() == "desc"
                    ? q => q.OrderByDescending(d => d.Name)
                    : q => q.OrderBy(d => d.Name),

                "description" => query.SortDir.ToLower() == "desc"
                    ? q => q.OrderByDescending(d => d.Description)
                    : q => q.OrderBy(d => d.Description),

                _ => query.SortDir.ToLower() == "desc"
                    ? q => q.OrderByDescending(d => d.Id)
                    : q => q.OrderBy(d => d.Id)
            };

        var includes = new Expression<Func<Department, object>>[]
        {
            d => d.Doctors
        };

        var result = await _unitOfWork.Departments.GetPagedAsync(
            query.PageNumber,
            query.PageSize,
            filter,
            orderBy,
            includes,
            cancellationToken);

        return new PagedResult<DepartmentDto>
        {
            Items = _mapper.Map<List<DepartmentDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<DepartmentDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var includes = new Expression<Func<Department, object>>[]
        {
            d => d.Doctors
        };

        var department = await _unitOfWork.Departments
            .GetByIdAsync(id, includes, cancellationToken);

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto> CreateAsync(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var department = _mapper.Map<Department>(dto);

        await _unitOfWork.Departments
            .AddAsync(department, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateDepartmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments
            .GetByIdAsync(id, null, cancellationToken);

        if (department is null)
            return false;

        _mapper.Map(dto, department);

        _unitOfWork.Departments.Update(department);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments
            .GetByIdAsync(id, null, cancellationToken);

        if (department is null)
            return false;

        _unitOfWork.Departments.Delete(department);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}