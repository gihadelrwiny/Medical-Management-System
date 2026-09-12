using Clinical.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<bool> HasConflictAsync(
            int doctorId,
            DateTime appointmentDate,
            int? excludeAppointmentId = null,
            CancellationToken cancellationToken = default);
    }
}
