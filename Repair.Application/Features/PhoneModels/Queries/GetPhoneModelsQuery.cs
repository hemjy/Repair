using MediatR;
using Repair.Application.Common;
using Repair.Application.DTOs.PhoneParts;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Repair.Application.Features.PhoneModels.Queries
{
    public class GetPhoneModelsQuery : PaginationRequest, IRequest<Result<List<GetPhoneModelsDto>>>
    {
        public Guid? BrandId { get; set; }
    }



    public class GetPhoneModelsQueryHandler(
            ILogger<GetPhoneModelsQueryHandler> logger,
            IGenericRepositoryAsync<PhoneModel> phoneModelRepository) : IRequestHandler<GetPhoneModelsQuery, Result<List<GetPhoneModelsDto>>>
    {

        public async Task<Result<List<GetPhoneModelsDto>>> Handle(GetPhoneModelsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = phoneModelRepository.GetAllQuery().Where(x => !x.IsDeleted);


                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    query = query.Where(x => x.Name.Trim().ToLower().Contains(request.SearchText.Trim().ToLower()));
                }
                if (request.Id.HasValue)
                {
                    query = query.Where(x => x.Id == request.Id);
                }
                if (request.BrandId.HasValue)
                { 
                    query = query.Where(x => x.BrandId == request.BrandId);
                }

               var total = query.Count();
                var data = query.Include(x => x.Brand).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                    .Select(x => new GetPhoneModelsDto { Name = x.Name, Id = x.Id, ImageUrl = x.ImageUrl, BrandId = x.BrandId, BrandName = x.Brand.Name })
                    .ToList();

                return Result<List<GetPhoneModelsDto>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting PhoneModels");
                return Result<List<GetPhoneModelsDto>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
