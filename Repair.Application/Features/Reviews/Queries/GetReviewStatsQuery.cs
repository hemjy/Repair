using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.PhoneParts;
using Repair.Application.DTOs.Reviews;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;

namespace Repair.Application.Features.PhoneParts.Queries
{
    public class GetReviewStatsQuery : IRequest<Result<GetReviewStat>>
    {
    }

    public class GetReviewStatsQueryHandler(
            ILogger<GetReviewStatsQueryHandler> logger,
            IGenericRepositoryAsync<Feedback> reviewRepo) : IRequestHandler<GetReviewStatsQuery, Result<GetReviewStat>>
    {

        public async Task<Result<GetReviewStat>> Handle(GetReviewStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await reviewRepo.GetAllQuery()
                 .Where(x => !x.IsDeleted)
                 .GroupBy(x => 1)
                 .Select(g => new
                 {
                     Count = g.Count(),
                     Total = g.Sum(x => x.Rating)
                 })
                 .FirstOrDefaultAsync();

                var count = result?.Count ?? 0;
                var total = result?.Total ?? 0;

                var response = new GetReviewStat
                {
                    Total = count,
                    AverageRating = count == 0 ? 0 :(float) Math.Round(total/count, 1),
                };
                return Result<GetReviewStat>.Success(response);
               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Reviews");
                return Result<GetReviewStat>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
