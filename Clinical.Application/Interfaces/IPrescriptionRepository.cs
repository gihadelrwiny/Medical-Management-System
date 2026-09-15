using Clinical.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.Interfaces
{

    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        Task<Prescription?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Prescription>> GetByMedicalRecordIdAsync(int medicalRecordId);
    }
}
