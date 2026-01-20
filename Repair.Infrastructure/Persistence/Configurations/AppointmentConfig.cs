using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Repair.Application.Helpers;
using Repair.Domain;
using Repair.Domain.Enums;

namespace Repair.Infrastructure.Persistence.Configurations
{
    internal class AppointmentConfig : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.Property(d => d.AppointmentStatus)
               .HasConversion(
                   v => v.ToString(),
                   v => EnumConverter.ConvertToEnum<AppointmentStatus>(v)
               );
        }
    }
}
