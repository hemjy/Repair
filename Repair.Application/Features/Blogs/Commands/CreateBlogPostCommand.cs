using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Blogs;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;

namespace Repair.Application.Features.Appointments.Commands
{
    public class CreateBlogPostValidator : AbstractValidator<CreateBlogPostCommand>
    {
        public CreateBlogPostValidator()
        {
            RuleFor(x => x.BlogCategoryId).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.Image).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.Title).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.Content).NotEmpty().WithMessage("{PropertyName} field is required");
           
        }


    }
    public class CreateBlogPostCommand : CreateBlogPostDto, IRequest<Result<Guid>>
    {
    }

    public class CreateBlogPostCommandHandler(ILogger<CreateBlogPostCommandHandler> logger,
            IGenericRepositoryAsync<BlogPost> postRepository,
            IGenericRepositoryAsync<BlogCategory> categoryRepository,
             IFileStorageService fileStorage
        ) : IRequestHandler<CreateBlogPostCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateBlogPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating Blog Post");

               var itemExist = await categoryRepository.IsUniqueAsync(x => request.BlogCategoryId == x.Id && !x.IsDeleted);
                if (!itemExist) return Result<Guid>.Failure("Category not Found.");
                var imageUrl = await fileStorage.UploadImageAsync(request.Image);
                var item = BlogPost.Create(request.Title, request.Content, imageUrl, request.BlogCategoryId, request.Status, request.IsFeatured);

                await postRepository.AddAsync(item);

                return Result<Guid>.Success(item.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during creating BlogPost");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
