using Clinical.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class Prescription : IEntity
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public int MedicalRecordId { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }


        // Navigation Properties

        public Patient Patient { get; set; } = null!;

        public Doctor Doctor { get; set; } = null!;

        public MedicalRecord MedicalRecord { get; set; } = null!;

        public ICollection<Medication> Medications { get; set; }
            = new List<Medication>();
    }
}
