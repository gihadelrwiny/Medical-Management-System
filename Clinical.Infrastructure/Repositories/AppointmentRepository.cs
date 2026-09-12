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
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicalDbContext context) : base(context)
        {
        }

        public async Task<bool> HasConflictAsync(
            int doctorId,
            DateTime appointmentDate,
            int? excludeAppointmentId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate == appointmentDate &&
                (excludeAppointmentId == null || a.Id != excludeAppointmentId),
                cancellationToken);
        }
    }
}
