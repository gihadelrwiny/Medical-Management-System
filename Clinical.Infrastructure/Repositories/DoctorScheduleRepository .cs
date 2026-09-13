using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using Clinical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Repositories
{

    public class DoctorScheduleRepository : Repository<DoctorSchedule>, IDoctorScheduleRepository
    {
        public DoctorScheduleRepository(ClinicalDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(
            int doctorId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ds => ds.DoctorId == doctorId && !ds.IsDeleted)
                .OrderBy(ds => ds.DayOfWeek)
                .ThenBy(ds => ds.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DoctorSchedule>> GetByDoctorAndDayAsync(
            int doctorId,
            DayOfWeek dayOfWeek,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ds => ds.DoctorId == doctorId
                             && ds.DayOfWeek == dayOfWeek
                             && !ds.IsDeleted)
                .OrderBy(ds => ds.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasOverlapAsync(
            int doctorId,
            DayOfWeek dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeScheduleId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ds => ds.DoctorId == doctorId
                             && ds.DayOfWeek == dayOfWeek
                             && !ds.IsDeleted
                             && (excludeScheduleId == null || ds.Id != excludeScheduleId))
                .AnyAsync(ds => startTime < ds.EndTime && endTime > ds.StartTime, cancellationToken);
        }
    }
}
