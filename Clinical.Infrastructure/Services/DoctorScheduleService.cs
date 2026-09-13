using AutoMapper;
using Clinical.Application.DTOs.DoctorSchedule;
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
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorScheduleService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<DoctorScheduleDto>> GetAllAsync(
            QueryParams query,
            CancellationToken cancellationToken = default)
        {
            Expression<Func<DoctorSchedule, bool>>? filter = null;

            // Search (matches doctor full name via navigation)
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                filter = ds =>
                    !ds.IsDeleted &&
                    (ds.Doctor.User.FirstName.Contains(search) ||
                     ds.Doctor.User.LastName.Contains(search));
            }
            else
            {
                filter = ds => !ds.IsDeleted;
            }

            // Sorting
            Func<IQueryable<DoctorSchedule>, IQueryable<DoctorSchedule>> orderBy =
                query.SortBy.ToLower() switch
                {
                    "day" => query.SortDir.ToLower() == "desc"
                        ? q => q.OrderByDescending(ds => ds.DayOfWeek)
                        : q => q.OrderBy(ds => ds.DayOfWeek),

                    "starttime" => query.SortDir.ToLower() == "desc"
                        ? q => q.OrderByDescending(ds => ds.StartTime)
                        : q => q.OrderBy(ds => ds.StartTime),

                    _ => query.SortDir.ToLower() == "desc"
                        ? q => q.OrderByDescending(ds => ds.Id)
                        : q => q.OrderBy(ds => ds.Id)
                };

            var includes = new Expression<Func<DoctorSchedule, object>>[]
            {
                ds => ds.Doctor
            };

            var result = await _unitOfWork.DoctorSchedules.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter,
                orderBy,
                includes,
                cancellationToken);

            return new PagedResult<DoctorScheduleDto>
            {
                Items = _mapper.Map<List<DoctorScheduleDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<DoctorScheduleDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var includes = new Expression<Func<DoctorSchedule, object>>[]
            {
                ds => ds.Doctor
            };

            var schedule = await _unitOfWork.DoctorSchedules
                .GetByIdAsync(id, includes, cancellationToken);

            if (schedule is null || schedule.IsDeleted)
                return null;

            return _mapper.Map<DoctorScheduleDto>(schedule);
        }

        public async Task<DoctorScheduleDto> CreateAsync(
            CreateDoctorScheduleDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be earlier than EndTime.");

            var hasOverlap = await _unitOfWork.DoctorSchedules.HasOverlapAsync(
                dto.DoctorId,
                dto.DayOfWeek,
                dto.StartTime,
                dto.EndTime,
                excludeScheduleId: null,
                cancellationToken: cancellationToken);

            if (hasOverlap)
                throw new InvalidOperationException("This schedule overlaps with an existing one for the doctor.");

            var schedule = _mapper.Map<DoctorSchedule>(dto);

            await _unitOfWork.DoctorSchedules
                .AddAsync(schedule, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DoctorScheduleDto>(schedule);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateDoctorScheduleDto dto,
            CancellationToken cancellationToken = default)
        {
            var schedule = await _unitOfWork.DoctorSchedules
                .GetByIdAsync(id, null, cancellationToken);

            if (schedule is null || schedule.IsDeleted)
                return false;

            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be earlier than EndTime.");

            var hasOverlap = await _unitOfWork.DoctorSchedules.HasOverlapAsync(
                schedule.DoctorId,
                dto.DayOfWeek,
                dto.StartTime,
                dto.EndTime,
                excludeScheduleId: id,
                cancellationToken: cancellationToken);

            if (hasOverlap)
                throw new InvalidOperationException("This schedule overlaps with an existing one for the doctor.");

            _mapper.Map(dto, schedule);

            _unitOfWork.DoctorSchedules.Update(schedule);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var schedule = await _unitOfWork.DoctorSchedules
                .GetByIdAsync(id, null, cancellationToken);

            if (schedule is null || schedule.IsDeleted)
                return false;

            schedule.IsDeleted = true;
            schedule.DeletedAt = DateTime.UtcNow;

            _unitOfWork.DoctorSchedules.Update(schedule);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
