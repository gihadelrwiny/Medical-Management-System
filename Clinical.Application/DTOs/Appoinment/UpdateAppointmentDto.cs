using Clinical.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.DTOs.Appoinment
{
    public class UpdateAppointmentDto
    {
        public DateTime AppointmentDate { get; set; }
        public string? Reason { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
