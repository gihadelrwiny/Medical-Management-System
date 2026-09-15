using Clinical.Application.DTOs.Medication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.DTOs.Prescription
{
    public class CreatePrescriptionDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int MedicalRecordId { get; set; }
        public List<CreateMedicationDto> Medications { get; set; } = new();
    }
}
