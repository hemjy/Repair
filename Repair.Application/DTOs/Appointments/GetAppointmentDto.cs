

using Repair.Domain;

namespace Repair.Application.DTOs.Appointments
{
    public class GetAppointmentDto
    {
        public Guid RepairPriceId { get; set; }
        public string Duration { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public Guid ModelId { get; set; }
        public string ModelName { get; set; }
        public string PartName { get; set; }
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
        public string Status { get; set; }
    }
}
