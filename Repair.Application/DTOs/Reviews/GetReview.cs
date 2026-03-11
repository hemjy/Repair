using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.DTOs.Reviews
{
    public class GetReview : CreateReviewDto
    {
        public Guid Id { get; set; }
      
        public string CustomerLastName { get; set; }
        public string CustomerFirstName { get; set; }
        public DateTime Created { get; set; }
    }

    public class GetReviewStat
    {
        public float AverageRating { get; set; }
        public long Total { get; set; }
    }
}
