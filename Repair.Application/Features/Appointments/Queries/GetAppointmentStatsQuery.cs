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
    public class GetAppointmentStatQuery :  IRequest<Result<GetAppointmentStatDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }



    public class GetAppointmentStatQueryHandler(
            ILogger<GetAppointmentStatQueryHandler> logger,
            IGenericRepositoryAsync<Appointment> appointmentRepository) : IRequestHandler<GetAppointmentStatQuery, Result<GetAppointmentStatDto>>
    {

        public async Task<Result<GetAppointmentStatDto>> Handle(GetAppointmentStatQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = appointmentRepository.GetAllQuery()
                    .Where(x => !x.IsDeleted);

                if (request.StartDate.HasValue)
                {
                    query = query.Where(x => x.Created >= request.StartDate.Value);
                }
                if (request.EndDate.HasValue)
                {
                    query = query.Where(x => x.Created <= request.EndDate.Value);
                }

                var stats = await query
                      .GroupBy(x => 1)
                      .Select(g => new GetAppointmentStatDto
                      {
                          CompletedRepair = g.Count(x =>
                              x.AppointmentStatus == AppointmentStatus.Done ||
                              x.AppointmentStatus == AppointmentStatus.Delivered),

                          TotalIncome = g
                              .Where(x =>
                                  x.AppointmentStatus == AppointmentStatus.Done ||
                                  x.AppointmentStatus == AppointmentStatus.Delivered)
                              .Sum(x => x.AmountPaid),

                          PendingIncome = g
                              .Where(x => x.AppointmentStatus == AppointmentStatus.Pending)
                              .Sum(x => x.AmountPaid),

                          PendingRepair = g.Count(x =>
                              x.AppointmentStatus == AppointmentStatus.Pending)
                      })
                      .FirstOrDefaultAsync();

                return Result<GetAppointmentStatDto>.Success(stats ?? new GetAppointmentStatDto());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Appointments");
                return Result<GetAppointmentStatDto>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
