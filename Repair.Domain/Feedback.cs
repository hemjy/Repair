

namespace Repair.Domain
{
    public class Feedback : EntityBase
    {
        private Feedback()
        {
            
        }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public static Feedback Create(Guid appointmentId, decimal rating, string comment) => new()
        {
            AppointmentId = appointmentId,
            Rating = rating,
            Comment = comment,
            Id = Guid.NewGuid()
        };
    }
}
