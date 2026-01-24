

using Microsoft.AspNetCore.Identity;

namespace Repair.Domain
{
    public class User : IdentityUser<Guid>
    {
        public bool IsActive { get; set; } = true;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
