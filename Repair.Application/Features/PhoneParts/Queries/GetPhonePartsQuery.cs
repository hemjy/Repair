using MediatR;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.PhoneParts;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;

namespace Repair.Application.Features.PhoneParts.Queries
{
    public class GetPhonePartsQuery : PaginationRequest, IRequest<Result<List<GetPhonePartsDto>>>
    {
    }

    public class GetPhonePartsQueryHandler(
            ILogger<GetPhonePartsQueryHandler> logger,
            IGenericRepositoryAsync<PhonePart> phonePartRepository) : IRequestHandler<GetPhonePartsQuery, Result<List<GetPhonePartsDto>>>
    {

        public async Task<Result<List<GetPhonePartsDto>>> Handle(GetPhonePartsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = phonePartRepository.GetAllQuery().Where(x => !x.IsDeleted);


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
                    .Select(x => new GetPhonePartsDto { Name = x.Name, Id = x.Id, ImageUrl = x.Image })
                    .ToList();

                return Result<List<GetPhonePartsDto>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting PhoneParts");
                return Result<List<GetPhonePartsDto>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
