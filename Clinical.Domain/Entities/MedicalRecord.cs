using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public  class MedicalRecord
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int AppointmentId { get; set; }

        public string Diagnosis { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public string? Treatment { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;


        // Navigation Properties

        public Patient Patient { get; set; } = null!;

        public Doctor Doctor { get; set; } = null!;

        public Appointment Appointment { get; set; } = null!;

        public ICollection<Prescription> Prescriptions { get; set; }
            = new List<Prescription>();
    }
}
