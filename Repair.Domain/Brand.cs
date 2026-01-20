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

        public static Brand Create(string Name) => new() { Name = Name };
    }
}
