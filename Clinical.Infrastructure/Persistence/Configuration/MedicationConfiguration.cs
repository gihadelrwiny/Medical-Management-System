using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Infrastructure.Persistence.Configuration
{

    public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
    {
        public void Configure(EntityTypeBuilder<Medication> builder)
        {
            builder.ToTable("Medications");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Dosage)
                .HasMaxLength(100);

            builder.Property(m => m.Frequency)
                .HasMaxLength(100);

            builder.Property(m => m.Duration)
                .HasMaxLength(100);

            builder.Property(m => m.Instructions)
                .HasMaxLength(1000);

            // Medications are owned by their prescription; if a
            // prescription is deleted (rare, admin-only action),
            // its line items should go with it.
            builder.HasOne(m => m.Prescription)
                .WithMany(p => p.Medications)
                .HasForeignKey(m => m.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
