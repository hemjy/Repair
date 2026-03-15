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
    public class GetCustomersQuery : PaginationRequest,  IRequest<Result<List<GetCustomerDto>>>
    {
       
    }



    public class GetCustomersQueryHandler(
            ILogger<GetCustomersQueryHandler> logger,
            IGenericRepositoryAsync<Appointment> appointmentRepository) : IRequestHandler<GetCustomersQuery, Result<List<GetCustomerDto>>>
    {

        public async Task<Result<List<GetCustomerDto>>> Handle(
     GetCustomersQuery request,
     CancellationToken cancellationToken)
        {
            try
            {
                var query = appointmentRepository
                    .GetAllQuery()
                    .Where(x => !x.IsDeleted);

                if (request.StartDate.HasValue)
                {
                    query = query.Where(x => x.Created >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    query = query.Where(x => x.Created <= request.EndDate.Value);
                }

                var customersQuery = query
                    .Select(x => new GetCustomerDto
                    {
                        FirstName = x.CustomerFirstName,
                        LastName = x.CustomerLastname,
                        Email = x.CustomerEmail,
                        PhoneNumber = x.CustomerPhoneNumber,
                        Address = x.CustomerAddress + " " + x.CustomerCity + " " + x.CustomerState
                    })
                    .Distinct();

                var total = await customersQuery.CountAsync(cancellationToken);

                var data = await customersQuery
                    .OrderBy(x => x.Email)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                return Result<List<GetCustomerDto>>.Success(
                    data,
                    request.PageNumber,
                    request.PageSize,
                    total);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Getting Customers");

                return Result<List<GetCustomerDto>>.Failure(
                    $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
