using MediatR;
using Repair.Application.Common;
using Repair.Application.DTOs.PhoneParts;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repair.Application.DTOs.Appointments;

namespace Repair.Application.Features.Appointments.Queries
{
    public class GetAppointmentsQuery : PaginationRequest, IRequest<Result<List<GetAppointmentDto>>>
    {
        public Guid? BrandId { get; set; }
        public Guid? ModelId { get; set; }
        public Guid? PartId { get; set; }
    }



    public class GetAppointmentsQueryHandler(
            ILogger<GetAppointmentsQueryHandler> logger,
            IGenericRepositoryAsync<Appointment> appointmentRepository) : IRequestHandler<GetAppointmentsQuery, Result<List<GetAppointmentDto>>>
    {

        public async Task<Result<List<GetAppointmentDto>>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = appointmentRepository.GetAllQuery()
                    .Include(x => x.RepairPrice.PhonePart)
                    .Include(x => x.RepairPrice.PhoneModel.Brand)
                    .Where(x => !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {

                    var search = $"%{request.SearchText.Trim().ToLower()}%";

                    query = query.Where(x =>
                        (x.Comment != null && EF.Functions.Like(x.Comment.ToLower(), search)) ||
                        (x.RepairPrice.PhoneModel.Name != null && EF.Functions.Like(x.RepairPrice.PhoneModel.Name.ToLower(), search)) ||
                        (x.RepairPrice.PhoneModel.Brand.Name != null && EF.Functions.Like(x.RepairPrice.PhoneModel.Brand.Name.ToLower(), search)) ||
                        (x.RepairPrice.PhonePart.Name != null && EF.Functions.Like(x.RepairPrice.PhonePart.Name.ToLower(), search))
                    );
                }
                if (request.Id.HasValue)
                {
                    query = query.Where(x => x.Id == request.Id);
                }
                if (request.BrandId.HasValue)
                { 
                    query = query.Where(x => x.RepairPrice.PhoneModel.BrandId == request.BrandId);
                }
                if (request.ModelId.HasValue)
                {
                    query = query.Where(x => x.RepairPrice.PhoneModelId == request.ModelId);
                }
                if (request.PartId.HasValue)
                {
                    query = query.Where(x => x.RepairPrice.PhonePartId == request.PartId);
                }

                var total = query.Count();
                var data = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                    .Select(x => new GetAppointmentDto { AppointmentTime = x.AppointmentTime.ToString("h:mm tt"),
                    Comment = x.Comment, AppointmentDay = x.AppointmentDay, BrandId = x.RepairPrice.PhoneModel.BrandId,
                    BrandName = x.RepairPrice.PhoneModel.Brand.Name,
                    CustomerAddress = x.CustomerAddress,
                    CustomerCity = x.CustomerCity, 
                    CustomerEmail = x.CustomerEmail,
                    CustomerFirstName = x.CustomerFirstName,
                    CustomerLastname = x.CustomerLastname,
                    CustomerPhoneNumber = x.CustomerPhoneNumber,
                    CustomerState = x.CustomerState,
                        Duration =
    $"{(x.RepairPrice.Duration / 60 > 0 ? $"{x.RepairPrice.Duration / 60} hour{(x.RepairPrice.Duration / 60 > 1 ? "s" : "")} " : "")}{x.RepairPrice.Duration % 60} minute{(x.RepairPrice.Duration % 60 != 1 ? "s" : "")}",
                        ModelId = x.RepairPrice.PhoneModelId,
                    ModelName = x.RepairPrice.PhoneModel.Name,
                    PartName = x.RepairPrice.PhonePart.Name,
                    RepairPriceId = x.RepairPriceId,
                    Status = x.AppointmentStatus.ToString()})
                    .ToList();

                return Result<List<GetAppointmentDto>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Appointments");
                return Result<List<GetAppointmentDto>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
