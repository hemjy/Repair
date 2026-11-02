using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Repair.Application.DTOs.Blogs;

namespace Repair.Application.Features.Appointments.Commands
{
    public class CreateBlogCategoryValidator : AbstractValidator<CreateBlogCategoryCommand>
    {
        public CreateBlogCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} field is required");
           
        }


    }
    public class CreateBlogCategoryCommand : CreateBlogCategoryDto, IRequest<Result<Guid>>
    {
    }

    public class CreateBlogCategoryCommandHandler(ILogger<CreateBlogCategoryCommandHandler> logger,
            IGenericRepositoryAsync<BlogCategory> categoryRepository) : IRequestHandler<CreateBlogCategoryCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating Blog Category");

               var itemExist = await categoryRepository.IsUniqueAsync(x => request.Name.Trim().ToLower() == x.Name.Trim().ToLower() && !x.IsDeleted);
                if (itemExist) return Result<Guid>.Failure("Category already exist.");

                var item = BlogCategory.Create(request.Name);

                await categoryRepository.AddAsync(item);

                return Result<Guid>.Success(item.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during creating BlogCategory");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
