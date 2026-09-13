using AutoMapper;
using Clinical.Application.DTOs.Appoinment;
using Clinical.Application.DTOs.Pagination;
using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using Clinical.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AppointmentDto>> GetAllAsync(
            QueryParams query,
            CancellationToken cancellationToken = default)
        {
            Expression<Func<Appointment, bool>>? filter = null;

            // Search (matches patient/doctor full name via navigation)
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                filter = a =>
                    a.Patient.User.FirstName.Contains(search) ||
                    a.Patient.User.LastName.Contains(search) ||
                    a.Doctor.User.FirstName.Contains(search) ||
                    a.Doctor.User.LastName.Contains(search) ||
                    (a.Reason != null && a.Reason.Contains(search));
            }

            // Sorting
            Func<IQueryable<Appointment>, IQueryable<Appointment>> orderBy =
                query.SortBy.ToLower() switch
                {
                    "date" => query.SortDir.ToLower() == "desc"
                        ? q => q.OrderByDescending(a => a.AppointmentDate)
                        : q => q.OrderBy(a => a.AppointmentDate),

                    "status" => query.SortDir.ToLower() == "desc"
                        ? q => q.OrderByDescending(a => a.Status)
                        : q => q.OrderBy(a => a.Status),

                    _ => query.SortDir.ToLower() == "desc"
                        ? q => q.OrderByDescending(a => a.Id)
                        : q => q.OrderBy(a => a.Id)
                };

            var includes = new Expression<Func<Appointment, object>>[]
            {
            a => a.Patient,
            a => a.Doctor
            };

            var result = await _unitOfWork.Appointments.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter,
                orderBy,
                includes,
                cancellationToken);

            return new PagedResult<AppointmentDto>
            {
                Items = _mapper.Map<List<AppointmentDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<AppointmentDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var includes = new Expression<Func<Appointment, object>>[]
            {
            a => a.Patient,
            a => a.Doctor
            };

            var appointment = await _unitOfWork.Appointments
                .GetByIdAsync(id, includes, cancellationToken);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto> CreateAsync(
     CreateAppointmentDto dto,
     CancellationToken cancellationToken = default)
        {
            await EnsureWithinDoctorScheduleAsync(dto.DoctorId, dto.AppointmentDate, cancellationToken);

            var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
                dto.DoctorId, dto.AppointmentDate, cancellationToken: cancellationToken);

            if (hasConflict)
                throw new InvalidOperationException("This doctor already has an appointment at that time.");

            var appointment = _mapper.Map<Appointment>(dto);
            appointment.Status = AppointmentStatus.Pending;

            await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AppointmentDto>(appointment);
        }
        public async Task<bool> UpdateAsync(
           int id,
           UpdateAppointmentDto dto,
           CancellationToken cancellationToken = default)
        {
            var appointment = await _unitOfWork.Appointments
                .GetByIdAsync(id, null, cancellationToken);

            if (appointment is null)
                return false;

            await EnsureWithinDoctorScheduleAsync(appointment.DoctorId, dto.AppointmentDate, cancellationToken);

            var hasConflict = await _unitOfWork.Appointments.HasConflictAsync(
                appointment.DoctorId, dto.AppointmentDate, excludeAppointmentId: id, cancellationToken: cancellationToken);

            if (hasConflict)
                throw new InvalidOperationException("This doctor already has an appointment at that time.");

            _mapper.Map(dto, appointment);
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var appointment = await _unitOfWork.Appointments
                .GetByIdAsync(id, null, cancellationToken);

            if (appointment is null)
                return false;

            _unitOfWork.Appointments.Delete(appointment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        private async Task EnsureWithinDoctorScheduleAsync(
    int doctorId,
    DateTime appointmentDate,
    CancellationToken cancellationToken)
        {
            var schedules = await _unitOfWork.DoctorSchedules.GetByDoctorAndDayAsync(
                doctorId, appointmentDate.DayOfWeek, cancellationToken);

            if (schedules is null || !schedules.Any())
                throw new InvalidOperationException(
                    $"Doctor is not available on {appointmentDate.DayOfWeek}.");

            var appointmentTime = appointmentDate.TimeOfDay;

            var isWithinAnySchedule = schedules.Any(s =>
                appointmentTime >= s.StartTime && appointmentTime <= s.EndTime);

            if (!isWithinAnySchedule)
            {
                var ranges = string.Join(", ", schedules
                    .OrderBy(s => s.StartTime)
                    .Select(s => $"{s.StartTime}-{s.EndTime}"));

                throw new InvalidOperationException(
                    $"Appointment time must fall within one of the doctor's available time ranges on {appointmentDate.DayOfWeek}: {ranges}.");
            }
        }

     
    }
}