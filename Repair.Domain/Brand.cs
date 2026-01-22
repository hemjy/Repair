using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Domain
{
    public sealed class Brand : EntityBase
    {
        private Brand()
        {
            
        }
        public required string Name { get; set; }
        public  string Description { get; set; }

        public static Brand Create(string Name, string description) => new() { Name = Name, Description = description};
    }
}
