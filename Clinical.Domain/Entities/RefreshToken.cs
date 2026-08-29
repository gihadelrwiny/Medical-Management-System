using Clinical.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Entities
{
    public class RefreshToken:IEntity
    {
        public int Id { get; set; }

        public required string Token { get; set; }

        public DateTime ExpiresOnUtc { get; set; }

        public DateTime? RevokedOnUtc { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public bool IsActive =>
            RevokedOnUtc is null &&
            DateTime.UtcNow < ExpiresOnUtc;
    }
}
