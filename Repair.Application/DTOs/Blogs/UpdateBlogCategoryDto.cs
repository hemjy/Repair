

using Repair.Domain.Enums;

namespace Repair.Application.DTOs.Blogs
{
    public class UpdateBlogCategoryDto : CreateBlogCategoryDto
    {
        public Guid Id { get; set; }
    }
    public class UpdateBlogPostDto : CreateBlogPostDto
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }
    }
}
