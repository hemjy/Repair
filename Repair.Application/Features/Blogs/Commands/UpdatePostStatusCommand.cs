using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Blogs;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Repair.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Repair.Application.Features.BlogPosts.Commands
{
    public class UpdateBlogPostStatusCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public BlogPostStatus Status { get; set; }

    }

    public class UpdateBlogPostStatusCommandHandler(ILogger<UpdateBlogPostCommandHandler> logger,
            IGenericRepositoryAsync<BlogPost> postRepository,
            IFileStorageService fileStorage) : IRequestHandler<UpdateBlogPostStatusCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateBlogPostStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating BlogPost");

                var post = await postRepository.GetByIdAsync(request.Id);
                if (post is null || post.IsDeleted) return Result<Guid>.Failure("Blog Post does not exit");
               
                post.Status = request.Status;
                post.Status = request.Status;

                // Add the new BlogPost to the repository
                await postRepository.UpdateAsync(post);
                return Result<Guid>.Success(post.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during updating BlogPost");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
