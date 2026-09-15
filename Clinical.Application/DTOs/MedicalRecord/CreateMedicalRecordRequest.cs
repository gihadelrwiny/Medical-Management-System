using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.DTOs.MedicalRecord
{
    public  class CreateMedicalRecordRequest
    {

        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Treatment { get; set; }
    }
}
