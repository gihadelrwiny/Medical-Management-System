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

    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Diagnosis)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.Notes)
                .HasMaxLength(4000);

            builder.Property(m => m.Treatment)
                .HasMaxLength(2000);

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            // One appointment produces at most one medical record
            builder.HasIndex(m => m.AppointmentId)
                .IsUnique();

            builder.HasOne(m => m.Patient)
                .WithMany()
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // One medical record has one prescription
            builder.HasOne(m => m.Prescription)
                .WithOne(p => p.MedicalRecord)
                .HasForeignKey<Prescription>(p => p.MedicalRecordId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
