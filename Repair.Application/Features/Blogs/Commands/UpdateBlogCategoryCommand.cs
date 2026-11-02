using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Repair.Application.DTOs.Blogs;

namespace Repair.Application.Features.Appointments.Commands
{
    public class UpdateBlogCategoryValidator : AbstractValidator<UpdateBlogCategoryCommand>
    {
        public UpdateBlogCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} field is required");
           
        }


    }
    public class UpdateBlogCategoryCommand : UpdateBlogCategoryDto, IRequest<Result<Guid>>
    {
    }

    public class UpdateBlogCategoryCommandHandler(ILogger<UpdateBlogCategoryCommandHandler> logger,
            IGenericRepositoryAsync<BlogCategory> categoryRepository) : IRequestHandler<UpdateBlogCategoryCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling editing Blog Category");

               var itemExist = await categoryRepository.IsUniqueAsync(x => x.Id != request.Id && request.Name.Trim().ToLower() == x.Name.Trim().ToLower() && !x.IsDeleted);
                if (itemExist) return Result<Guid>.Failure("Category Name already exist.");

                var item = await categoryRepository.GetByIdAsync(request.Id);
                if (item is null) return Result<Guid>.Failure("Category does not exist");
                item.Name = request.Name;
                await categoryRepository.UpdateAsync(item);

                return Result<Guid>.Success(item.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during editing BlogCategory");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
