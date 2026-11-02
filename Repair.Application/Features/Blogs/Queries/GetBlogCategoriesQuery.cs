using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Blogs;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;

namespace Repair.Application.Features.PhoneParts.Queries
{
    public class GetBlogCategoriesQuery : PaginationRequest, IRequest<Result<List<GetBlogCategoryDto>>>
    {
    }

    public class GetBlogCategoriesQueryHandler(
            ILogger<GetBlogCategoriesQueryHandler> logger,
            IGenericRepositoryAsync<BlogCategory> categoryRepo,
            IGenericRepositoryAsync<BlogPost> postRepo
        ) : IRequestHandler<GetBlogCategoriesQuery, Result<List<GetBlogCategoryDto>>>
    {
        public async Task<Result<List<GetBlogCategoryDto>>> Handle(
            GetBlogCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var categoriesQuery = categoryRepo
                    .GetAllQuery()
                    .Where(x => !x.IsDeleted);

                // Search
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    var searchText = request.SearchText.Trim().ToLower();

                    categoriesQuery = categoriesQuery.Where(x =>
                        x.Name.ToLower().Contains(searchText));
                }

                // Filter by Id
                if (request.Id.HasValue)
                {
                    categoriesQuery = categoriesQuery
                        .Where(x => x.Id == request.Id.Value);
                }

                // Get total before pagination
                var total = await categoriesQuery.CountAsync(cancellationToken);

                var data = await categoriesQuery
                    .OrderBy(x => x.Name)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new GetBlogCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Created = x.Created,

                        PostCount = postRepo
                            .GetAllQuery()
                            .Count(p =>
                                !p.IsDeleted &&
                                p.BlogCategoryId == x.Id)
                    })
                    .ToListAsync(cancellationToken);

                return Result<List<GetBlogCategoryDto>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error during Getting BlogCategories");

                return Result<List<GetBlogCategoryDto>>.Failure(
                    $"An unexpected error occurred: {ex.Message}");
            }
        }

    }
}
