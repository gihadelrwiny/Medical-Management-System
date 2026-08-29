using Clinical.Application.Interfaces;
using Clinical.Domain.Entities;
using Clinical.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Repositories
{
    public class DoctorRepository
       : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicalDbContext context)
            : base(context)
        {
        }
    }
}
