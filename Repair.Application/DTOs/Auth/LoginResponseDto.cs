using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public Guid UserId { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }
    }
}
