using Clinical.Application.DTOs.Medication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.DTOs.Prescription
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int MedicalRecordId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MedicationDto> Medications { get; set; } = new();
    }
}
