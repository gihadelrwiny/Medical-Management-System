using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Domain.Common.Interfaces
{
   public interface IEntity
    {
        public int Id{ get; set; }
        bool IsDeleted { get; set; }

        DateTime? DeletedAt { get; set; }
    }
}
