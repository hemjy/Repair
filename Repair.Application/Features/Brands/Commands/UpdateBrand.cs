using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System.ComponentModel.DataAnnotations;

namespace Repair.Application.Features.Brands.Commands
{
    public class UpdateBrandCommand : IRequest<Result<Guid>>
    {

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string? Name { get; set; }
        [Required]
        public Guid Id { get; set; }

       
    }

    public class UpdateBrandCommandHandler(ILogger<UpdateBrandCommandHandler> logger,
            IGenericRepositoryAsync<Brand> brandRepository) : IRequestHandler<UpdateBrandCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating Brand");

                var brandExists = await brandRepository
                                   .IsUniqueAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower() && !x.IsDeleted && x.Id != request.Id);

                if (brandExists)
                {
                    return Result<Guid>.Failure("Brand Already Exist.");
                }
                var brand = await brandRepository.GetByIdAsync(request.Id);
                if (brand is null || brand.IsDeleted) return Result<Guid>.Failure("Brand does not exit");
                brand.Name = request.Name;

                // Add the new brand to the repository
                await brandRepository.UpdateAsync(brand);
                return Result<Guid>.Success(brand.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during updating brand");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
