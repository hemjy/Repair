

using Microsoft.AspNetCore.Identity;

namespace Repair.Domain
{
    public class Role : IdentityRole<Guid>
    {
        private Role() { }
        public static Role Create(string name) =>new() { Name = name };
    }
}
