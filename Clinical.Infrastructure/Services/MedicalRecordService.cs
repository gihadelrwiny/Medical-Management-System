using AutoMapper;
using Clinical.Application.Common;
using Clinical.Application.DTOs.MedicalRecord;
using Clinical.Application.DTOs.Pagination;
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
    public class MedicalRecordService(
     IUnitOfWork unitOfWork,
     IMapper mapper) : IMedicalRecordService
    {
        public async Task<PagedResult<MedicalRecordDto>> GetAllAsync(
            QueryParams query,
            CancellationToken ct)
        {
            Expression<Func<MedicalRecord, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                filter = m =>
                    m.Diagnosis.Contains(search) ||
                    (m.Treatment != null && m.Treatment.Contains(search));
            }

            Func<IQueryable<MedicalRecord>, IQueryable<MedicalRecord>> orderBy =
                query.SortBy.ToLower() switch
                {
                    "diagnosis" => q => query.SortDir.ToLower() == "desc"
                        ? q.OrderByDescending(m => m.Diagnosis)
                        : q.OrderBy(m => m.Diagnosis),

                    "createdat" => q => query.SortDir.ToLower() == "desc"
                        ? q.OrderByDescending(m => m.CreatedAt)
                        : q.OrderBy(m => m.CreatedAt),

                    _ => q => query.SortDir.ToLower() == "desc"
                        ? q.OrderByDescending(m => m.Id)
                        : q.OrderBy(m => m.Id)
                };

            var result = await unitOfWork.MedicalRecords.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter,
                orderBy,
                new Expression<Func<MedicalRecord, object>>[]
                {
                m => m.Patient,
                m => m.Doctor,
                m => m.Appointment
                },
                ct);

            return new PagedResult<MedicalRecordDto>
            {
                Items = mapper.Map<List<MedicalRecordDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<MedicalRecordDto?> GetByIdAsync(
            int id,
            CancellationToken ct)
        {
            var record = await unitOfWork.MedicalRecords
                .GetByIdAsync(id, cancellationToken: ct);

            return mapper.Map<MedicalRecordDto>(record);
        }

        public async Task<Result<MedicalRecordDto>> CreateAsync(
            CreateMedicalRecordRequest request,
            CancellationToken ct)
        {
            bool patientExists = await unitOfWork.Patients
                .ExistsAsync(p => p.Id == request.PatientId, ct);

            if (!patientExists)
                return Result<MedicalRecordDto>.Failure("Patient not found.");

            bool doctorExists = await unitOfWork.Doctors
                .ExistsAsync(d => d.Id == request.DoctorId, ct);

            if (!doctorExists)
                return Result<MedicalRecordDto>.Failure("Doctor not found.");

            bool appointmentExists = await unitOfWork.Appointments
                .ExistsAsync(a => a.Id == request.AppointmentId, ct);

            if (!appointmentExists)
                return Result<MedicalRecordDto>.Failure("Appointment not found.");

            var record = mapper.Map<MedicalRecord>(request);

            await unitOfWork.MedicalRecords.AddAsync(record, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var dto = mapper.Map<MedicalRecordDto>(record);

            return Result<MedicalRecordDto>.Success(dto);
        }

        public async Task UpdateAsync(
            int id,
            UpdateMedicalRecordDto dto,
            CancellationToken ct)
        {
            var existing = await unitOfWork.MedicalRecords
                .GetByIdAsync(id, cancellationToken: ct);

            if (existing is null)
                throw new KeyNotFoundException("Medical record not found.");

            mapper.Map(dto, existing);

            unitOfWork.MedicalRecords.Update(existing);

            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken ct)
        {
            var existing = await unitOfWork.MedicalRecords
                .GetByIdAsync(id, cancellationToken: ct);

            if (existing is null)
                throw new KeyNotFoundException("Medical record not found.");

            // Soft delete, since the entity has IsDeleted/DeletedAt
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;

            unitOfWork.MedicalRecords.Update(existing);

            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}
