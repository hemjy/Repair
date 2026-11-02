using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Blogs;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System.ComponentModel.DataAnnotations;

namespace Repair.Application.Features.BlogPosts.Commands
{
    public class UpdateBlogPostCommand : UpdateBlogPostDto, IRequest<Result<Guid>>
    {

       
    }

    public class UpdateBlogPostCommandHandler(ILogger<UpdateBlogPostCommandHandler> logger,
            IGenericRepositoryAsync<BlogPost> postRepository,
             IGenericRepositoryAsync<BlogCategory> categoryRepository,
            IFileStorageService fileStorage) : IRequestHandler<UpdateBlogPostCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateBlogPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating BlogPost");
                var itemExist = await categoryRepository.IsUniqueAsync(x => request.BlogCategoryId == x.Id && !x.IsDeleted);
                if (!itemExist) return Result<Guid>.Failure("Invalid Category Selected.");

                var post = await postRepository.GetByIdAsync(request.Id);
                if (post is null || post.IsDeleted) return Result<Guid>.Failure("BlogPost does not exit");
                if(request.Image is not null)
                {
                    var imageUrl = await fileStorage.UploadImageAsync(request.Image);
                    post.ImageUrl = imageUrl;
                }
                post.Title = request.Title;
                post.Content = request.Content;
                post.BlogCategoryId = request.BlogCategoryId;
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
