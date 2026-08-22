using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }


        // Navigation Property

        public ICollection<Doctor> Doctors { get; set; }
            = new List<Doctor>();
    }
}
