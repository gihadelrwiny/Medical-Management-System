using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class DoctorSchedule
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;


        // Navigation Property

        public Doctor Doctor { get; set; } = null!;
    }
}
