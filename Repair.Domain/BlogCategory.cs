using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Domain
{
    public sealed class BlogCategory : EntityBase
    {
        private BlogCategory()
        {
            
        }
        public required string Name { get; set; }

        public static BlogCategory Create(string Name) => new() { Name = Name};
    }
}
