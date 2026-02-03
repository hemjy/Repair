

using Repair.Domain;

namespace Repair.Application.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        public Guid RepairPriceId { get; set; }
        public string Comment { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastname { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerCity { get; set; }
        public string? CustomerState { get; set; }
        public DateTime AppointmentDay { get; set; }
        public string AppointmentTime { get; set; }
    }
}
