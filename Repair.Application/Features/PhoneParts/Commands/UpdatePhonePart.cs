using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System.ComponentModel.DataAnnotations;

namespace Repair.Application.Features.PhoneParts.Commands
{
    public class UpdatePhonePartCommand : IRequest<Result<Guid>>
    {

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public Guid Id { get; set; }

       
    }

    public class UpdatePhonePartCommandHandler(ILogger<UpdatePhonePartCommandHandler> logger,
            IGenericRepositoryAsync<PhonePart> phonePartRepository,
            IFileStorageService fileStorage) : IRequestHandler<UpdatePhonePartCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdatePhonePartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating PhonePart");

                var PhonePartExists = await phonePartRepository
                                   .IsUniqueAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower() && !x.IsDeleted && x.Id != request.Id);

                if (PhonePartExists)
                {
                    return Result<Guid>.Failure("PhonePart Already Exist.");
                }
                var phonePart = await phonePartRepository.GetByIdAsync(request.Id);
                if (phonePart is null || phonePart.IsDeleted) return Result<Guid>.Failure("PhonePart does not exit");
                if (request.Image is not null) 
                {
                    var imageUrl = await fileStorage.UploadImageAsync(request.Image);
                    phonePart.Image = imageUrl;
                }
              
                phonePart.Name = request.Name;
               

                // Add the new PhonePart to the repository
                await phonePartRepository.UpdateAsync(phonePart);
                return Result<Guid>.Success(phonePart.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during updating PhonePart");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
