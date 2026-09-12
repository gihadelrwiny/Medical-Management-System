using AutoMapper;
using Clinical.Application.Common;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.DTOs.Patients;
using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Services
{
    public class PatientService(
     IUnitOfWork unitOfWork,
     IMapper mapper) : IPatientService
    {
        public async Task<PagedResult<PatientDto>> GetAllAsync(
            QueryParams query,
            CancellationToken ct)
        {
            Expression<Func<Patient, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                filter = p =>
                    p.User.FirstName.Contains(search) ||
                    p.User.LastName.Contains(search) ||
                    p.PhoneNumber.Contains(search);
            }

            Func<IQueryable<Patient>, IQueryable<Patient>> orderBy =
                query.SortBy.ToLower() switch
                {
                    "firstname" => q => query.SortDir.ToLower() == "desc"
                        ? q.OrderByDescending(p => p.User.FirstName)
                        : q.OrderBy(p => p.User.FirstName),

                    "lastname" => q => query.SortDir.ToLower() == "desc"
                        ? q.OrderByDescending(p => p.User.LastName)
                        : q.OrderBy(p => p.User.LastName),

                    "phonenumber" => q => query.SortDir.ToLower() == "desc"
                        ? q.OrderByDescending(p => p.PhoneNumber)
                        : q.OrderBy(p => p.PhoneNumber),

                    _ => q => query.SortDir.ToLower() == "desc"
                        ? q.OrderByDescending(p => p.Id)
                        : q.OrderBy(p => p.Id)
                };

            var result = await unitOfWork.Patients.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter,
                orderBy,
                new Expression<Func<Patient, object>>[]
                {
                p => p.User
                },
                ct);

            return new PagedResult<PatientDto>
            {
                Items = mapper.Map<List<PatientDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }


        public async Task<PatientDto?> GetByIdAsync(
            int id,
            CancellationToken ct)
        {
            var patient = await unitOfWork.Patients
                .GetByIdAsync(id, cancellationToken: ct);

            return mapper.Map<PatientDto>(patient);
        }

        public async Task UpdateAsync(
            int id,
            UpdatePatientDto dto,
            CancellationToken ct)
        {
            var existingPatient = await unitOfWork.Patients
                .GetByIdAsync(id, cancellationToken: ct);

            if (existingPatient is null)
                throw new KeyNotFoundException("Patient not found.");

            mapper.Map(dto, existingPatient);

            unitOfWork.Patients.Update(existingPatient);

            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<Result<bool>> DeleteAsync(
             int id,
           CancellationToken ct)
        {
            var patient = await unitOfWork.Patients.GetByIdAsync(
                id,
                new Expression<Func<Patient, object>>[]
                {
            p => p.User
                },
                ct);

            if (patient is null)
            {
                return Result<bool>.Failure("Patient not found.");
            }

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                // Delete Patient
                unitOfWork.Patients.Delete(patient);

                // Delete the related User
                if (patient.User is not null)
                {
                    unitOfWork.Users.Delete(patient.User);
                }

                await unitOfWork.SaveChangesAsync(ct);

                await unitOfWork.CommitTransactionAsync(ct);

                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(ct);

                return Result<bool>.Failure(
                    "Failed to delete patient.");
            }
        }
    }
}