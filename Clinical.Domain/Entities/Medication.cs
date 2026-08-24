using Clinical.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public  class Medication : IEntity
    {
        public int Id { get; set; }

        public int PrescriptionId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Dosage { get; set; }

        public string? Frequency { get; set; }

        public string? Duration { get; set; }

        public string? Instructions { get; set; }


        // Navigation Property

        public Prescription Prescription { get; set; } = null!;
    }
}
