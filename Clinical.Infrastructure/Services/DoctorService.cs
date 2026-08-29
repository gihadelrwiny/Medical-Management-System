using AutoMapper;
using Clinical.Application.Common;
using Clinical.Application.DTOs.Doctor;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using Clinical.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Clinical.Infrastructure.Services;

public class DoctorService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPasswordHasher<User> passwordHasher) : IDoctorService   // <-- injected here
{
    public async Task<PagedResult<DoctorDto>> GetAllAsync(
        QueryParams query,
        CancellationToken ct)
    {
        Expression<Func<Doctor, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            filter = d =>
                d.User.FirstName.Contains(search) ||
                d.User.LastName.Contains(search) ||
                d.PhoneNumber.Contains(search);
        }

        Func<IQueryable<Doctor>, IQueryable<Doctor>> orderBy =
            query.SortBy.ToLower() switch
            {
                "firstname" => q => query.SortDir.ToLower() == "desc"
                    ? q.OrderByDescending(d => d.User.FirstName)
                    : q.OrderBy(d => d.User.FirstName),

                "lastname" => q => query.SortDir.ToLower() == "desc"
                    ? q.OrderByDescending(d => d.User.LastName)
                    : q.OrderBy(d => d.User.LastName),

                "phonenumber" => q => query.SortDir.ToLower() == "desc"
                    ? q.OrderByDescending(d => d.PhoneNumber)
                    : q.OrderBy(d => d.PhoneNumber),

                _ => q => query.SortDir.ToLower() == "desc"
                    ? q.OrderByDescending(d => d.Id)
                    : q.OrderBy(d => d.Id)
            };

        var result = await unitOfWork.Doctors.GetPagedAsync(
            query.PageNumber,
            query.PageSize,
            filter,
            orderBy,
            new Expression<Func<Doctor, object>>[]
            {
                d => d.User,
                d => d.Department
            },
            ct);

        return new PagedResult<DoctorDto>
        {
            Items = mapper.Map<List<DoctorDto>>(result.Items),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync(
        CancellationToken ct)
    {
        var doctors = await unitOfWork.Doctors
            .GetAllAsync(cancellationToken: ct);

        return mapper.Map<IEnumerable<DoctorDto>>(doctors);
    }

    public async Task<DoctorDto?> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var doctor = await unitOfWork.Doctors
            .GetByIdAsync(id, cancellationToken: ct);

        return mapper.Map<DoctorDto>(doctor);
    }

    public async Task<Result<DoctorDto>> CreateAsync(
        CreateDoctorRequest request,
        CancellationToken ct)
    {
        string email = request.Email
            .Trim()
            .ToLowerInvariant();

        bool emailTaken = await unitOfWork.Users
            .ExistsAsync(u => u.Email == email, ct);

        if (emailTaken)
        {
            return Result<DoctorDto>.Failure(
                "This email is already registered.");
        }

        bool departmentExists = await unitOfWork.Departments
            .ExistsAsync(d => d.Id == request.DepartmentId, ct);

        if (!departmentExists)
        {
            return Result<DoctorDto>.Failure(
                "Department not found.");
        }

        string[] nameParts = request.FullName
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string firstName = nameParts.FirstOrDefault() ?? string.Empty;

        string lastName = nameParts.Length > 1
            ? string.Join(' ', nameParts.Skip(1))
            : string.Empty;

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = string.Empty,
            Role = UserRole.Doctor
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            // 1. Create User
            await unitOfWork.Users.AddAsync(user, ct);

            // Save first so User.Id is generated
            await unitOfWork.SaveChangesAsync(ct);

            // 2. Create Doctor using generated User.Id
            var doctor = new Doctor
            {
                UserId = user.Id,
                DepartmentId = request.DepartmentId,
                PhoneNumber = request.PhoneNumber
            };

            await unitOfWork.Doctors.AddAsync(doctor, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Commit transaction
            await unitOfWork.CommitTransactionAsync(ct);

            var doctorDto = mapper.Map<DoctorDto>(doctor);

            return Result<DoctorDto>.Success(doctorDto);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(ct);

            return Result<DoctorDto>.Failure(
                "Doctor registration failed. Please try again.");
        }
    }

    public async Task UpdateAsync(
        int id,
        UpdateDoctorDto dto,
        CancellationToken ct)
    {
        var existingDoctor = await unitOfWork.Doctors
            .GetByIdAsync(id, cancellationToken: ct);

        if (existingDoctor is null)
            throw new KeyNotFoundException("Doctor not found.");

        mapper.Map(dto, existingDoctor);

        unitOfWork.Doctors.Update(existingDoctor);

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken ct)
    {
        var doctor = await unitOfWork.Doctors
            .GetByIdAsync(id, cancellationToken: ct);

        if (doctor is null)
            throw new KeyNotFoundException("Doctor not found.");

        unitOfWork.Doctors.Delete(doctor);

        await unitOfWork.SaveChangesAsync(ct);
    }
}