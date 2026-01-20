using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.Features.Prices.Commands
{
    public class UpdatePriceCommand : IRequest<Result<Guid>>
    {
        [Required]
        public Guid PhoneModelId { get; set; }
        [Required]
        public Guid PhonePartId { get; set; }
        public decimal Cost { get; set; }
        public Guid Id { get; set; }

       
    }

    public class UpdatePriceCommandHandler(ILogger<UpdatePriceCommandHandler> logger,
             IGenericRepositoryAsync<RepairPrice> priceRepository,
             IGenericRepositoryAsync<PhoneModel> phoneModelRepository,
              IGenericRepositoryAsync<PhonePart> phonePartRepository) : IRequestHandler<UpdatePriceCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdatePriceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling updating Price");

                var priceExists = await priceRepository.IsUniqueAsync(x => x.PhoneModelId == request.PhoneModelId && x.PhonePartId == request.PhonePartId && !x.IsDeleted && x.Id != request.Id);
                var phoneModelExists = await phoneModelRepository.IsUniqueAsync(x => x.Id == request.PhoneModelId && !x.IsDeleted);
                var phonePartExists = await phonePartRepository.IsUniqueAsync(x => x.Id == request.PhonePartId && !x.IsDeleted);
               
                if (priceExists) return Result<Guid>.Failure("Price Already Exist for the Phone Part");
                if (!phoneModelExists) return Result<Guid>.Failure("Invalid Phone Model Selected");
                if (!phonePartExists) return Result<Guid>.Failure("Invalid Phone Part Selected");
               
                var price = await priceRepository.GetByIdAsync(request.Id);
                if (price is null || price.IsDeleted) return Result<Guid>.Failure("Price does not exit");
               
                price.PhonePartId = request.PhonePartId;
                price.PhoneModelId = request.PhoneModelId;
                price.Cost = request.Cost;

                // update Price to the repository
                await priceRepository.UpdateAsync(price);
                return Result<Guid>.Success(price.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during updating Price");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
