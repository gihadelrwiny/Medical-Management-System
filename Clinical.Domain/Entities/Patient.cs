using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public  class Patient
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();
    }
}
