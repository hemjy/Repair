using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Brands;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;

namespace Repair.Application.Features.Brands.Queries
{
    public class GetBrandsQuery : PaginationRequest, IRequest<Result<List<GetBrandsDto>>>
    {
    }

   


    public class GetBrandsQueryHandler(
            ILogger<GetBrandsQueryHandler> logger,
            IGenericRepositoryAsync<Brand> brandRepository) : IRequestHandler<GetBrandsQuery, Result<List<GetBrandsDto>>>
    {

        public async Task<Result<List<GetBrandsDto>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = brandRepository.GetAllQuery().Where(x => !x.IsDeleted);


                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    query = query.Where(x => x.Name.Trim().ToLower().Contains(request.SearchText.Trim().ToLower()));
                }
                if (request.Id.HasValue)
                {
                    query = query.Where(x => x.Id == request.Id);
                }

               var total = query.Count();
                var data = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                    .Select(x => new GetBrandsDto { Name = x.Name, Id = x.Id })
                    .ToList();

                return Result<List<GetBrandsDto>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Brands");
                return Result<List<GetBrandsDto>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
