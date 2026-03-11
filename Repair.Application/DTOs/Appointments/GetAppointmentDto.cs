

using Repair.Domain;

namespace Repair.Application.DTOs.Appointments
{
    public class GetAppointmentDto
    {
        public Guid Id { get; set; }
        public Guid RepairPriceId { get; set; }
        public decimal Cost { get; set; }
        public string Duration { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public string OrderNumber { get; set; }
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

    public class GetAppointmentStatDto
    {
        public decimal TotalIncome { get; set; }
        public decimal PendingIncome { get; set; }
        public int CompletedRepair { get; set; }
        public int PendingRepair { get; set; }
    }
    public class GetTopRepairPartDto
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Part { get; set; }
        public int Count { get; set; }
        public decimal Cost { get; set; }
    }
    public class GetTopCustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal TotalCost { get; set; }
        public string Email { get; set; }
        public int Count { get; set; }
        public string PhoneNumber { get; set; }
    }
}
