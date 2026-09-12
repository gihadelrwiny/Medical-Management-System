using Clinical.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class Doctor : IEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public User User { get; set; } = null!;

        public Department Department { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();
    }
}
