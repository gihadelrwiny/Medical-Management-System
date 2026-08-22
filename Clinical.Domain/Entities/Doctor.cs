using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        public Department Department { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();
    }
}
