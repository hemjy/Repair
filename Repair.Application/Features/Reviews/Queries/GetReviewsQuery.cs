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
    public class GetReviewsQuery : PaginationRequest, IRequest<Result<List<GetReview>>>
    {
    }

    public class GetReviewsQueryHandler(
            ILogger<GetReviewsQueryHandler> logger,
            IGenericRepositoryAsync<Feedback> reviewRepo) : IRequestHandler<GetReviewsQuery, Result<List<GetReview>>>
    {

        public async Task<Result<List<GetReview>>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = reviewRepo.GetAllQuery().Where(x => !x.IsDeleted);


                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    query = query.Where(x => x.Comment.Trim().ToLower().Contains(request.SearchText.Trim().ToLower()));
                }
                if (request.Id.HasValue)
                {
                    query = query.Where(x => x.Id == request.Id);
                }

               var total = query.Count();
                var data = query.Include(x => x.Appointment)
                    .OrderByDescending(x => x.Rating)
                    .ThenByDescending(x => x.Created)
                    .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                    .Select(x => new GetReview { Comment = x.Comment, Id = x.Id, Rating = x.Rating, CustomerFirstName = x.Appointment.CustomerFirstName, CustomerLastName = x.Appointment.CustomerLastname,
                    Created = x.Created})
                    .ToList();

                return Result<List<GetReview>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Reviews");
                return Result<List<GetReview>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
