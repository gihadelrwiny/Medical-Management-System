using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Application.DTOs.Doctor
{
    public class CreateDoctorRequest
    {
        [Required]
    [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
