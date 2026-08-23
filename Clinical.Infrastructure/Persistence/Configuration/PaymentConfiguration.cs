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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            // Explicit precision so it isn't left to provider defaults
            // (or a startup warning) on a money column.
            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Method)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

            // Avoid two payments recorded under the same gateway
            // transaction id (only enforced when a value is present).
            builder.HasIndex(p => p.TransactionId)
                .IsUnique()
                .HasFilter("[TransactionId] IS NOT NULL");

            builder.Property(p => p.PaidAt);

            // One payment per appointment
            builder.HasIndex(p => p.AppointmentId)
                .IsUnique();

            builder.HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                // Never lose billing/payment history when a patient is removed.
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment <-> Payment FK/uniqueness declared here as the
            // dependent side; also configured on AppointmentConfiguration.
        }
    }
}
