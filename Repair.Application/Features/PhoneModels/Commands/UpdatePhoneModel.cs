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
    public class UpdatePhoneModelCommand : IRequest<Result<Guid>>
    {

        [Required]
        [StringLength(200, ErrorMessage = "Name cannot be longer than 200 characters.")]
        public string? Name { get; set; }
        public string? IMageUrl { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid BrandId { get; set; }

       
    }

    public class UpdatePhoneModelCommandHandler(ILogger<UpdatePhoneModelCommandHandler> logger,
            IGenericRepositoryAsync<PhoneModel> phoneModelRepository,
             IGenericRepositoryAsync<Brand> brandRepository,
            IFileStorageService fileStorage) : IRequestHandler<UpdatePhoneModelCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdatePhoneModelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating PhoneModel");

                var phoneModelExists = await phoneModelRepository
                                   .IsUniqueAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower() && x.BrandId == request.BrandId && !x.IsDeleted && x.Id != request.Id);
                var brandExists = await brandRepository
                                   .IsUniqueAsync(x => x.Id == request.BrandId && !x.IsDeleted);

                if (phoneModelExists)
                {
                    return Result<Guid>.Failure("Inavlid Brand Selected.");
                }
                if(!brandExists) return Result<Guid>.Failure("B Already Exist.");
                var phoneModel = await phoneModelRepository.GetByIdAsync(request.Id);
                if (phoneModel is null || phoneModel.IsDeleted) return Result<Guid>.Failure("PhoneModel does not exit");
                if(request.Image is not null)
                {
                    var imageUrl = await fileStorage.UploadImageAsync(request.Image);
                    phoneModel.ImageUrl = imageUrl;
                }
                phoneModel.Name = request.Name;

                // Add the new PhoneModel to the repository
                await phoneModelRepository.UpdateAsync(phoneModel);
                return Result<Guid>.Success(phoneModel.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during updating PhoneModel");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
