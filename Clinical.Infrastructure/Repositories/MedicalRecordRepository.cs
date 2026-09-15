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
    public class MedicalRecordRepository
        : Repository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(ClinicalDbContext context)
            : base(context)
        {
        }
    }
}
