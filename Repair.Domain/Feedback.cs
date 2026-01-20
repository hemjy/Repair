

namespace Repair.Domain
{
    public class Feedback : EntityBase
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}
