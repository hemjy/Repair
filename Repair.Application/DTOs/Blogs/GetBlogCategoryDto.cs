

using Repair.Domain.Enums;

namespace Repair.Application.DTOs.Blogs
{
    public class GetBlogCategoryDto : UpdateBlogCategoryDto
    {
        public DateTime Created { get; set; }
        public int PostCount { get; set; }
    }
    public class GetBlogPostDto : UpdateBlogPostDto
    {
        public DateTime Created { get; set; }
        public new string Status { get; set; }
        public string Category { get; set; }
    }
}
