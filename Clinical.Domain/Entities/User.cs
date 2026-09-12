using Clinical.Domain.Common.Interfaces;
using Clinical.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public UserRole Role { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    = new List<RefreshToken>();
    }
}
