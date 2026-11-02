using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Blogs;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Repair.Domain.Enums;

namespace Repair.Application.Features.PhoneParts.Queries
{
    public class GetBlogPostsQuery : PaginationRequest, IRequest<Result<List<GetBlogPostDto>>>
    {
        public BlogPostStatus? Status { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsFeatured { get; set; }
    }

    public class GetBlogPostsQueryHandler(
            ILogger<GetBlogPostsQueryHandler> logger,
            IGenericRepositoryAsync<BlogPost> postRepo) : IRequestHandler<GetBlogPostsQuery, Result<List<GetBlogPostDto>>>
    {

        public async Task<Result<List<GetBlogPostDto>>> Handle(GetBlogPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = postRepo.GetAllQuery()
                    .Where(x => !x.IsDeleted);


                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    query = query.Where(x => x.Content.Trim().ToLower().Contains(request.SearchText.Trim().ToLower()));
                }
                if (request.Id.HasValue)
                {
                    query = query.Where(x => x.Id == request.Id);
                }
                if (request.StartDate.HasValue)
                {
                    query = query.Where(x => x.Created >= request.StartDate);
                }
                if (request.EndDate.HasValue)
                {
                    query = query.Where(x => x.Created <= request.EndDate);
                }
                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status);
                }
                if (request.CategoryId.HasValue)
                {
                    query = query.Where(x => x.BlogCategoryId == request.CategoryId);
                }
                if (request.IsFeatured.HasValue)
                {
                    query = query.Where(x => x.IsFeatured == request.IsFeatured);
                }

                var total = query.Count();
                var data = query.Include(x => x.BlogCategory)
                    .OrderBy(x => x.Created)
                    .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                    .Select(x => new GetBlogPostDto { Id = x.Id,Content = x.Content, ImageUrl = x.ImageUrl, Title = x.Title,
                    Status = x.Status.ToString(),BlogCategoryId = x.BlogCategoryId, Category = x.BlogCategory.Name,
                    IsFeatured = x.IsFeatured,
                    Created = x.Created})
                    .ToList();

                return Result<List<GetBlogPostDto>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting BlogPosts");
                return Result<List<GetBlogPostDto>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
