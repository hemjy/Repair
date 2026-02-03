

using Repair.Domain.Enums;

namespace Repair.Domain
{
    public sealed class Appointment : EntityBase
    {
        private Appointment()
        {
            
        }
        public Guid RepairPriceId { get; set; }
        public RepairPrice RepairPrice { get; set; }
        public decimal AmountPaid { get; set; }
        public string Comment { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastname { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerCity { get; set; }
        public string? CustomerState { get; set; }
        public DateTime AppointmentDay { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }

        public static Appointment Create(Guid repairPriceId, string comment, string customerPhoneNumber, string customerLastname, string customerFirstName, string customerEmail, TimeOnly appointmentTime, DateTime appointmentDay, string? CustomerState, string? CustomerCity, string? CustomerAddress) => new()
        {
            RepairPriceId = repairPriceId,
            CustomerPhoneNumber = customerPhoneNumber,
            CustomerLastname = customerLastname,
            CustomerFirstName = customerFirstName,
            Id = Guid.NewGuid(),
            CustomerEmail = customerEmail,
            AppointmentDay = appointmentDay,
            AppointmentStatus = AppointmentStatus.Pending,
            AppointmentTime = appointmentTime,
            AmountPaid = 0,
            Comment = comment,
            CustomerAddress = CustomerAddress,
            CustomerCity = CustomerCity,
            CustomerState = CustomerState,
            
        };
       
    }
}
