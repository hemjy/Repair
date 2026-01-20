

using Repair.Domain.Enums;

namespace Repair.Domain
{
    public sealed class Appointment : EntityBase
    {
        public Guid PhonePartId { get; set; }
        public PhonePart PhonePart { get; set; }
        public decimal AmountPaid { get; set; }
        public string Comment { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastname { get; set; }
        public string? OwnerAddress { get; set; }
        public string? OwnerCity { get; set; }
        public string? OwnerState { get; set; }
        public DateTime AppointmentDay { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
       
    }
}
