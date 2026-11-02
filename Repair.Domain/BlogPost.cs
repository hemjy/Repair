using Repair.Domain.Enums;

namespace Repair.Domain
{
    public sealed class BlogPost : EntityBase
    {
        public Guid BlogCategoryId { get; set; }
        public BlogCategory BlogCategory { get; set; }
        public required bool  IsFeatured { get; set; }
        public required string  Title { get; set; }
        public required string Content { get; set; }
        public required BlogPostStatus Status { get; set; }
        public required string ImageUrl { get; set; }

        public static BlogPost Create(string title, string content, string imageUrl, Guid categoryId, BlogPostStatus status, bool isFeatured) => new() { IsFeatured = isFeatured, Status = status, Content = content, ImageUrl = imageUrl, Title = title, BlogCategoryId = categoryId};
    }
}
