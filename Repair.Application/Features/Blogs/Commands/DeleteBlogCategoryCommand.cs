using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Microsoft.EntityFrameworkCore;

namespace Repair.Application.Features.Appointments.Commands
{
    public class DeleteBlogCategoryCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public bool DeleteAnyway { get; set; }
    }

    public class DeleteBlogCategoryCommandHandler(
        ILogger<DeleteBlogCategoryCommandHandler> logger,
            IGenericRepositoryAsync<BlogCategory> categoryRepository,
            IGenericRepositoryAsync<BlogPost> postRepository
        ) : IRequestHandler<DeleteBlogCategoryCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(DeleteBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling deleting Blog Category");
                var item = await categoryRepository.GetByIdAsync(request.Id);
                if (item is null) return Result<Guid>.Failure("Category does not exist");
                var posts = await postRepository.GetAllQuery().Where(x => x.BlogCategoryId == request.Id).ToListAsync();
                if(posts != null &&  posts.Count > 0)
                {
                    if (request.DeleteAnyway)
                    {
                        await postRepository.DeleteAsync(posts, false);
                    }
                    else
                    {
                        return Result<Guid>.Failure($"You have {posts.Count} Post{(posts.Count > 1 ? "s" : "")} attached to the Category");
                    }
                }
                
                await categoryRepository.DeleteAsync(item);

                return Result<Guid>.Success(item.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during deleting BlogCategory");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
