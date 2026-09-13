using Clinical.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{
   
        public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
        {
            Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(
                int doctorId,
                CancellationToken cancellationToken = default);

            Task<IEnumerable<DoctorSchedule>> GetByDoctorAndDayAsync(
                int doctorId,
                DayOfWeek dayOfWeek,
                CancellationToken cancellationToken = default);

            Task<bool> HasOverlapAsync(
                int doctorId,
                DayOfWeek dayOfWeek,
                TimeSpan startTime,
                TimeSpan endTime,
                int? excludeScheduleId = null,
                CancellationToken cancellationToken = default);
        }
    }

