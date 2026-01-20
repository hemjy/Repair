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
    public class CreatePhonePartCommand : IRequest<Result<Guid>>
    {

        [Required]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters.")]
        public string? Name { get; set; }
        public IFormFile Image { get; set; }

       
    }

    public class CreatePhonePartCommandHandler(ILogger<CreatePhonePartCommandHandler> logger,
            IGenericRepositoryAsync<PhonePart> PhonePartRepository, IFileStorageService fileStorageService) : IRequestHandler<CreatePhonePartCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreatePhonePartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating PhonePart");

                var phonePartExists = await PhonePartRepository
                                   .IsUniqueAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower() && !x.IsDeleted);

                if (phonePartExists)
                {
                    return Result<Guid>.Failure("Phone Part Already Exist.");
                }

                var ImageUrl = await fileStorageService.UploadImageAsync(request.Image);
                // Create a new PhonePart 
                var newPhonePart = PhonePart
                    .Create(request.Name, ImageUrl);

                // Add the new PhonePart to the repository
                await PhonePartRepository.AddAsync(newPhonePart);
                return Result<Guid>.Success(newPhonePart.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during creating PhonePart");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
