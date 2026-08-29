using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.DTOs.Doctor
{
    public class UpdateDoctorDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
