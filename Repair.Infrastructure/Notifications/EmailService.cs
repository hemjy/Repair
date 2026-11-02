using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Repair.Application.DTOs.EmailSetting;
using Repair.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace Repair.Infrastructure.Notifications
{
    public class EmailService(
        IOptions<Google> googleConfig,
        IOptions<BrevoOptions> brevoOptions,
        HttpClient httpClient,
    ILogger<EmailService> logger) : IEmailService
    {
        public async Task<bool> SendAsync(EmailMessage request)
        {
            return await Google(request);
        }
        private async Task<bool> Google(EmailMessage request)
        {


            var message = new MimeMessage();
            foreach (var email in request.Recipients)
            {
                if (!string.IsNullOrWhiteSpace(email))
                {
                    message.To.Add(MailboxAddress.Parse(email.Trim()));
                }
            }
            message.From.Add(new MailboxAddress(googleConfig.Value.EmailSenderName, googleConfig.Value.EmailSenderAddress));
            message.Subject = request.Subject;

            message.Body = new TextPart("html")
            {
                Text = request.Body
            };

            var multipart = new Multipart("mixed")
            {
                message.Body
            };
            if (request.EmailAttachments != null)
            {
                foreach (var attachment in request.EmailAttachments)
                {
                    var attachmentPart = new MimePart(attachment.ContentType)
                    {
                        Content = new MimeContent(new MemoryStream(Convert.FromBase64String(attachment.Base64Content))),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = attachment.FileName
                    };
                    multipart.Add(attachmentPart);
                }
            }

            message.Body = multipart;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    logger.LogInformation("Connecting to SMTP server at {SMTPServerAddress}", googleConfig.Value.SMTPServerAddress);
                    client.Connect(googleConfig.Value.SMTPServerAddress, googleConfig.Value.SMTPServerPort, true);
                    client.Authenticate(googleConfig.Value.EmailSenderAddress, googleConfig.Value.EmailSenderPassword);
                    client.Send(message);
                    logger.LogInformation("Email sent successfully to: {Email}", string.Join(',', request.Recipients));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while sending email to: {Email}", string.Join(',', request.Recipients));
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }

            }
            return true;
        }

     

            private async Task BrevoAsync(
                string recipientEmail,
                string recipientName,
                string subject,
                string htmlContent,
                CancellationToken cancellationToken = default)
            {
                var requestBody = new
                {
                    sender = new
                    {
                        name = brevoOptions.Value.SenderName,
                        email = brevoOptions.Value.SenderEmail
                    },

                    to = new[]
                    {
                new
                {
                    email = recipientEmail,
                    name = recipientName
                }
            },

                    subject,
                    htmlContent
                };

                var json = JsonSerializer.Serialize(requestBody);

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "v3/smtp/email");

                request.Headers.Add("api-key", brevoOptions.Value.ApiKey);

                request.Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                using var response = await httpClient.SendAsync(
                    request,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(
                        cancellationToken);

                    throw new Exception(
                        $"Brevo email failed. Status: {response.StatusCode}. Error: {error}");
                }
            }
        
    }
}
