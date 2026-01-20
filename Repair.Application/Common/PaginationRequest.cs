namespace Repair.Application.Common
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 
        public Guid? Id { get; set; }
        public string? SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public PaginationRequest(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
