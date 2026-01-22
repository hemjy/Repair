using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System.ComponentModel.DataAnnotations;

namespace Repair.Application.Features.Brands.Commands
{
    public class CreateBrandCommand : IRequest<Result<Guid>>
    {

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }

       
    }

    public class CreateBrandCommandHandler(ILogger<CreateBrandCommandHandler> logger,
            IGenericRepositoryAsync<Brand> brandRepository) : IRequestHandler<CreateBrandCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating Brand");

                var brandExists = await brandRepository
                                   .IsUniqueAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower() && !x.IsDeleted);

                if (brandExists)
                {
                    return Result<Guid>.Failure("Brand Already Exist.");
                }

                // Create a new brand 
                var newbrand = Brand
                    .Create(request.Name, request.Description);

                // Add the new brand to the repository
                await brandRepository.AddAsync(newbrand);
                return Result<Guid>.Success(newbrand.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during creating brand");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
