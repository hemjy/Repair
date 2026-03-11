using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.DTOs.EmailSetting
{
    public class Google
    {
        public string EmailSenderAddress { get; set; }
        public string EmailSenderName { get; set; }
        public string EmailSenderPassword { get; set; }
        public string SMTPServerAddress { get; set; }
        public int SMTPServerPort { get; set; }
        public bool SMTPServerEnableSSL { get; set; }
    }
}
