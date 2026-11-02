

using Microsoft.AspNetCore.Http;
using Repair.Domain.Enums;

namespace Repair.Application.DTOs.Blogs
{
    public class CreateBlogPostDto
    {
        public Guid BlogCategoryId { get; set; }
        public BlogPostStatus Status { get; set; }
        public bool IsFeatured { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}
