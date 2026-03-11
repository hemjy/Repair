using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.DTOs.Reviews
{
    public class CreateReviewDto
    {
        public string OrderNumber { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }

    }
}
