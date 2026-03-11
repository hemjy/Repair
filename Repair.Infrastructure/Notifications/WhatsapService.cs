using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repair.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Infrastructure.Notifications
{
    public class WhatsapService(IConfiguration configuration) : IWhatsapService
    {
        public string GetWhatsAppLink(string message)
        {
            var encodedMsg = Uri.EscapeDataString(message);
            return $"https://wa.me/{configuration["WhatsappNumber"]}?text={encodedMsg}";
        }
    }
}
