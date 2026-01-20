using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System.ComponentModel.DataAnnotations;

namespace Repair.Application.Features.Prices.Commands
{
    public class CreatePriceCommand : IRequest<Result<Guid>>
    {

        [Required]
        public Guid PhoneModelId { get; set; }
        [Required]
        public Guid PhonePartId { get; set; }
        public decimal Cost { get; set; }
        [Required]
        public int Duration { get; set; }

       
    }

    public class CreatePriceCommandHandler(ILogger<CreatePriceCommandHandler> logger,
            IGenericRepositoryAsync<RepairPrice> priceRepository,
             IGenericRepositoryAsync<PhoneModel> phoneModelRepository,
              IGenericRepositoryAsync<PhonePart> phonePartRepository
            ) : IRequestHandler<CreatePriceCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreatePriceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating Price");

                var priceExists = await priceRepository.IsUniqueAsync(x => x.PhoneModelId == request.PhoneModelId && x.PhonePartId == request.PhonePartId && !x.IsDeleted);
                var phoneModelExists = await phoneModelRepository.IsUniqueAsync(x => x.Id == request.PhoneModelId && !x.IsDeleted);
                var phonePartExists = await phonePartRepository.IsUniqueAsync(x =>  x.Id == request.PhonePartId && !x.IsDeleted);

                if (priceExists) return Result<Guid>.Failure("Price Already Exist for the Phone Part");
                if (!phoneModelExists) return Result<Guid>.Failure("Invalid Phone Model Selected");
                if (!phonePartExists) return Result<Guid>.Failure("Invalid Phone Part Selected");
               

                // Create a new Price 
                var newPrice = RepairPrice
                    .Create(request.PhoneModelId, request.PhonePartId, request.Cost, request.Duration);

                // Add the new Price to the repository
                await priceRepository.AddAsync(newPrice);
                return Result<Guid>.Success(newPrice.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during creating Price");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
