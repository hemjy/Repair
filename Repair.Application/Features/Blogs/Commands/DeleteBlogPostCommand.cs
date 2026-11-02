using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Repair.Application.DTOs.Blogs;
using Microsoft.EntityFrameworkCore;

namespace Repair.Application.Features.Appointments.Commands
{
    public class DeleteBlogPostCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteBlogPostCommandHandler(
        ILogger<DeleteBlogPostCommandHandler> logger,
            IGenericRepositoryAsync<BlogPost> PostRepository,
            IGenericRepositoryAsync<BlogPost> postRepository
        ) : IRequestHandler<DeleteBlogPostCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(DeleteBlogPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling deleting Blog Post");
                var item = await PostRepository.GetByIdAsync(request.Id);
                if (item is null) return Result<Guid>.Failure("Post does not exist");
                
                await postRepository.DeleteAsync(item);

                return Result<Guid>.Success(item.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during deleting BlogPost");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
