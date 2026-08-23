using Clinical.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string? Reason { get; set; }

        public AppointmentStatus Status { get; set; }
            = AppointmentStatus.Pending;


        // Navigation Properties

        public Patient Patient { get; set; } = null!;

        public Doctor Doctor { get; set; } = null!;

        public MedicalRecord? MedicalRecord { get; set; }

        public Payment? Payment { get; set; }
    }
}
