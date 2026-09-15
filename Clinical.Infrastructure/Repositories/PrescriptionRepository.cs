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
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        private readonly ClinicalDbContext _context;

        public PrescriptionRepository(ClinicalDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Prescription?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Include(p => p.Medications.Where(m => !m.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.Medications.Where(m => !m.IsDeleted))
                .Where(p => p.PatientId == patientId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Medications.Where(m => !m.IsDeleted))
                .Where(p => p.DoctorId == doctorId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetByMedicalRecordIdAsync(int medicalRecordId)
        {
            return await _context.Prescriptions
                .Include(p => p.Medications.Where(m => !m.IsDeleted))
                .Where(p => p.MedicalRecordId == medicalRecordId && !p.IsDeleted)
                .ToListAsync();
        }
    }
}
