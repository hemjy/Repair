using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System.ComponentModel.DataAnnotations;

namespace Repair.Application.Features.PhoneModels.Commands
{
    public class CreatePhoneModelCommand : IRequest<Result<Guid>>
    {

        [Required]
        [StringLength(200, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string? Name { get; set; }
        [Required]
        public Guid BrandId { get; set; }
        [Required]
        public IFormFile Image { get; set; }

       
    }

    public class CreatePhoneModelCommandHandler(ILogger<CreatePhoneModelCommandHandler> logger,
            IGenericRepositoryAsync<PhoneModel> phoneModelRepository,
            IGenericRepositoryAsync<Brand> brandRepository,
            IFileStorageService fileStorage) : IRequestHandler<CreatePhoneModelCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreatePhoneModelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating PhoneModel");

                var phoneModelExists = await phoneModelRepository
                                   .IsUniqueAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower() && x.BrandId == request.BrandId && !x.IsDeleted);
                var brandExist = await brandRepository.IsUniqueAsync(x => x.Id == request.BrandId && !x.IsDeleted);
                if (phoneModelExists)
                {
                    return Result<Guid>.Failure("Phone Model Already Exist.");
                }
                if (!brandExist) return Result<Guid>.Failure("Phone Brand Does not exist.");
               

                var imageUrl = await fileStorage.UploadImageAsync(request.Image);
                // Create a new PhoneModel 
                var newPhoneModel = PhoneModel
                    .Create(request.Name, imageUrl, request.BrandId);

                // Add the new PhoneModel to the repository
                await phoneModelRepository.AddAsync(newPhoneModel);
                return Result<Guid>.Success(newPhoneModel.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during creating PhoneModel");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
