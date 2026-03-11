

namespace Repair.Domain
{
    public class ContactMessage : EntityBase
    {
        public string Message { get; set; }
        public string Subject { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
    }
}
