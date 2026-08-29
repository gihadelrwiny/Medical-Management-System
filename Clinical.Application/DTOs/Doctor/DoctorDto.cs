using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.DTOs.Doctor
{
    public class DoctorDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
