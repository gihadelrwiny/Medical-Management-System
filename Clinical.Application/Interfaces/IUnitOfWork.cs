using Clinical.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Patient> Patients { get; }
        IRepository<Doctor> Doctors { get; }
        IRepository<Department> Departments { get; }
        IRepository<DoctorSchedule> DoctorSchedules { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<MedicalRecord> MedicalRecords { get; }
        IRepository<Prescription> Prescriptions { get; }
        IRepository<Medication> Medications { get; }
        IRepository<Payment> Payments { get; }
        IRepository<Notification> Notifications { get; }

        /// <summary>
        /// Persists all pending changes across every repository used in
        /// this unit of work, in a single DbContext SaveChanges call
        /// (i.e. a single transaction unless an explicit one is started).
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts an explicit DB transaction spanning multiple
        /// SaveChangesAsync calls. Use only when a unit of work needs to
        /// commit in stages (e.g. an ID generated mid-flow is needed for
        /// a second insert) — a single SaveChangesAsync call is already
        /// atomic on its own and does not need this.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}