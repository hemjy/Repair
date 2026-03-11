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
    public class GetTopRepairPartsQuery : PaginationRequest,  IRequest<Result<List<GetTopRepairPartDto>>>
    {
       
    }



    public class GetTopRepairPartsQueryHandler(
            ILogger<GetTopRepairPartsQueryHandler> logger,
            IGenericRepositoryAsync<Appointment> appointmentRepository) : IRequestHandler<GetTopRepairPartsQuery, Result<List<GetTopRepairPartDto>>>
    {

        public async Task<Result<List<GetTopRepairPartDto>>> Handle(GetTopRepairPartsQuery request, CancellationToken cancellationToken)
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
                        PartName = x.RepairPrice.PhonePart.Name,
                        ModelName = x.RepairPrice.PhoneModel.Name,
                        BrandName = x.RepairPrice.PhoneModel.Brand.Name
                    });

                var projectedQuery = groupedQuery
                    .Select(g => new GetTopRepairPartDto
                    {
                        Part = g.Key.PartName,
                        Model = g.Key.ModelName,
                        Brand = g.Key.BrandName,
                        Count = g.Count(),
                        Cost = g.Sum(x => x.AmountPaid)
                    });

                var total = await projectedQuery.CountAsync();

                var data = await projectedQuery
                    .OrderByDescending(x => x.Count)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();
                return Result<List<GetTopRepairPartDto>>.Success(
                 data,
                 request.PageNumber,
                 request.PageSize,
                 total);


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Appointments");
                return Result<List<GetTopRepairPartDto>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
