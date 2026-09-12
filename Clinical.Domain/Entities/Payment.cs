using Clinical.Domain.Common.Interfaces;
using Clinical.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class Payment : IEntity
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int AppointmentId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }
            = PaymentStatus.Pending;

        public string? TransactionId { get; set; }

        public DateTime? PaidAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }


        // Navigation Properties

        public Patient Patient { get; set; } = null!;

        public Appointment Appointment { get; set; } = null!;
    }
}
