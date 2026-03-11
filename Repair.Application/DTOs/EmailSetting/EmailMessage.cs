

using System.Text.Json.Serialization;

namespace Repair.Application.DTOs.EmailSetting
{
    public class EmailMessage
    {
        public required string Body { get; set; }
        public required string Subject { get; set; }
        public required List<string> Recipients { get; set; }
        public List<EmailAttachment>? EmailAttachments { get; set; }
    }
    public class EmailAttachment
    {
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public required string Base64Content { get; set; }

        [JsonIgnore] public byte[] Content => Convert.FromBase64String(Base64Content);

    }
    public class MailSettings
    {
        public required string From { get; set; }
        public required string DisplayName { get; set; }
    }
}
