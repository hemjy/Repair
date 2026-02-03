

using Repair.Domain;
using Repair.Domain.Enums;

namespace Repair.Application.DTOs.Appointments
{
    public class UpdateAppointmentDto
    {
        public Guid Id { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
    }
}
