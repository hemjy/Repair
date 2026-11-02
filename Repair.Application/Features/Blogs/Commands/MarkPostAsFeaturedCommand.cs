using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class MarkPostAsFeaturedCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }

    }

    public class MarkPostAsFeaturedCommandHandler(ILogger<UpdateBlogPostCommandHandler> logger,
            IGenericRepositoryAsync<BlogPost> postRepository,
            IFileStorageService fileStorage) : IRequestHandler<MarkPostAsFeaturedCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(MarkPostAsFeaturedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating BlogPost");

                var post = await postRepository.GetByIdAsync(request.Id);
                if (post is null || post.IsDeleted) return Result<Guid>.Failure("Blog Post does not exit");
               
                post.IsFeatured = true;

                // Add the new BlogPost to the repository
                var affectedRows = await postRepository.GetAllQuery()
                        .Where(x => x.Id != request.Id && x.IsFeatured && x.BlogCategoryId == post.BlogCategoryId)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(x => x.IsFeatured, false),
                            cancellationToken);
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
