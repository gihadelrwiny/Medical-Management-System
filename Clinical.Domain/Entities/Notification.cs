using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;


        // Navigation Property

        public Patient Patient { get; set; } = null!;
    }
}
