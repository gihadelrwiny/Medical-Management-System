using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using Clinical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicalDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        // Backing fields — created lazily, on first access, and reused
        // for the lifetime of this UnitOfWork instance (one per request
        // in a typical scoped DI lifetime).
        private IRepository<User>? _users;
        private IRepository<Patient>? _patients;
        private IDoctorRepository? _doctors;
        private IRepository<Department>? _departments;
        private IDoctorScheduleRepository _doctorSchedules;
        private IAppointmentRepository? _appointments;
        private IMedicalRecordRepository? _medicalRecords;
        private IRepository<Prescription>? _prescriptions;
        private IRepository<Medication>? _medications;
        private IRepository<Payment>? _payments;
        private IRepository<Notification>? _notifications;
        private IRepository<RefreshToken>? _refreshTokens;

        public UnitOfWork(ClinicalDbContext context)
        {
            _context = context;
        }

        public IRepository<User> Users =>
            _users ??= new Repository<User>(_context);

        public IRepository<Patient> Patients =>
            _patients ??= new Repository<Patient>(_context);

        public IDoctorRepository Doctors =>
      _doctors ??= new DoctorRepository(_context);

        public IRepository<Department> Departments =>
            _departments ??= new Repository<Department>(_context);

        public IDoctorScheduleRepository DoctorSchedules =>
            _doctorSchedules ??= new DoctorScheduleRepository(_context);


        public IAppointmentRepository Appointments =>
            _appointments ??= new AppointmentRepository(_context);

        public IMedicalRecordRepository MedicalRecords =>
            _medicalRecords ??= new MedicalRecordRepository (_context);

        public IRepository<Prescription> Prescriptions =>
            _prescriptions ??= new Repository<Prescription>(_context);

        public IRepository<Medication> Medications =>
            _medications ??= new Repository<Medication>(_context);

        public IRepository<Payment> Payments =>
            _payments ??= new Repository<Payment>(_context);

        public IRepository<Notification> Notifications =>
            _notifications ??= new Repository<Notification>(_context);
        public IRepository<RefreshToken> RefreshTokens =>
      _refreshTokens ??= new Repository<RefreshToken>(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is not null)
            {
                // Avoid silently nesting/replacing an in-flight transaction
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
            {
                return;
            }

            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_currentTransaction is not null)
            {
                await _currentTransaction.DisposeAsync();
            }

            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}