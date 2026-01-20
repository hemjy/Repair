using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.PhoneParts;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;

namespace Repair.Application.Features.Prices.Queries
{
    public class GetPricesQuery : PaginationRequest, IRequest<Result<List<GetPricesDto>>>
    {
        public Guid? PhoneModelId { get; set; }
        public Guid? BrandId { get; set; }
       
        public Guid? PhonePartId { get; set; }
        public decimal Cost { get; set; }
    }

   


    public class GetPricesQueryHandler(
            ILogger<GetPricesQueryHandler> logger,
            IGenericRepositoryAsync<RepairPrice> priceRepository) : IRequestHandler<GetPricesQuery, Result<List<GetPricesDto>>>
    {

        public async Task<Result<List<GetPricesDto>>> Handle(GetPricesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = priceRepository.GetAllQuery().Where(x => !x.IsDeleted || !x.PhonePart.IsDeleted || !x.PhoneModel.IsDeleted || !x.PhoneModel.Brand.IsDeleted);


                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    query = query.Where(x => x.PhonePart.Name.Trim().ToLower().Contains(request.SearchText.Trim().ToLower()));
                }
                if (request.Id.HasValue)
                {
                    query = query.Where(x => x.Id == request.Id);
                }
                if (request.PhonePartId.HasValue)
                {
                    query = query.Where(x => x.PhonePartId == request.PhonePartId);
                }
                if (request.PhoneModelId.HasValue)
                {
                    query = query.Where(x => x.PhoneModelId == request.PhoneModelId);
                }
                if (request.BrandId.HasValue)
                {
                    query = query.Where(x => x.PhoneModel.BrandId == request.BrandId);
                }

                var total = query.Count();
                var data = query.Include(x => x.PhonePart).Include(x => x.PhoneModel.Brand).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                    .Select(x => new GetPricesDto {  Id = x.Id, BrandId = x.PhoneModel.BrandId, BrandName = x.PhoneModel.Brand.Name,
                    PhoneModelId = x.PhoneModelId, PhoneModeName = x.PhoneModel.Name, PhonePartId = x.PhonePartId, Cost = x.Cost, PhonePartName = x.PhonePart.Name, DUration = $"{x.Duration} Minutes"})
                    .ToList();

                return Result<List<GetPricesDto>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Prices");
                return Result<List<GetPricesDto>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
