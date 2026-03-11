using MediatR;
using Repair.Application.Common;
using Repair.Application.DTOs.PhoneParts;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repair.Application.DTOs.Appointments;
using Repair.Domain.Enums;

namespace Repair.Application.Features.Appointments.Queries
{
    public class GetTopCustomersQuery : PaginationRequest,  IRequest<Result<List<GetTopCustomerDto>>>
    {
       
    }



    public class GetTopCustomersQueryHandler(
            ILogger<GetTopCustomersQueryHandler> logger,
            IGenericRepositoryAsync<Appointment> appointmentRepository) : IRequestHandler<GetTopCustomersQuery, Result<List<GetTopCustomerDto>>>
    {

        public async Task<Result<List<GetTopCustomerDto>>> Handle(GetTopCustomersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var baseQuery = appointmentRepository
      .GetAllQuery()
      .Where(x => !x.IsDeleted);

                if (request.StartDate.HasValue)
                {
                    baseQuery = baseQuery.Where(x => x.Created >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    baseQuery = baseQuery.Where(x => x.Created <= request.EndDate.Value);
                }

                var groupedQuery = baseQuery
                    .GroupBy(x => new
                    {
                       x.CustomerEmail,
                       x.CustomerFirstName,
                       x.CustomerLastname,
                       x.CustomerPhoneNumber
                    });

                var projectedQuery = groupedQuery
                    .Select(g => new GetTopCustomerDto
                    {
                       Name = $"{g.Key.CustomerFirstName}  {g.Key.CustomerLastname}",
                        Count = g.Count(),
                        TotalCost = g.Sum(x => x.AmountPaid),
                        Email = g.Key.CustomerEmail,
                        PhoneNumber = g.Key.CustomerPhoneNumber
                    });

                var total = await projectedQuery.CountAsync();

                var data = await projectedQuery
                    .OrderByDescending(x => x.Count)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();
                return Result<List<GetTopCustomerDto>>.Success(
                 data,
                 request.PageNumber,
                 request.PageSize,
                 total);


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Appointments");
                return Result<List<GetTopCustomerDto>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
