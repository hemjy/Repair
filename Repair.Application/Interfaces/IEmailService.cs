

using Repair.Application.DTOs.EmailSetting;

namespace Repair.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAsync(EmailMessage request);
    }
}
