using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Appointments;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Repair.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.Features.Appointments.Commands
{
    public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.AppointmentStatus).NotEmpty().NotEqual(AppointmentStatus.None).WithMessage("{PropertyName} field is required");

        }


    }
    public class UpdateAppointmentCommand : UpdateAppointmentDto, IRequest<Result<Guid>>
    {
    }

    public class UpdateAppointmentCommandHandler(ILogger<UpdateAppointmentCommandHandler> logger,
            IGenericRepositoryAsync<Appointment> appointmentRepository) : IRequestHandler<UpdateAppointmentCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling updating Appointment");

               var item = await appointmentRepository.GetByAsync(x => x.Id == request.Id && !x.IsDeleted, [x => x.RepairPrice]);
                if (item == null) return Result<Guid>.Failure("No Appointment Found.");

                if(item.AppointmentStatus == Domain.Enums.AppointmentStatus.Delivered) return Result<Guid>.Failure("Item/Service has been completed.");
               item.AppointmentStatus = request.AppointmentStatus;
                item.AmountPaid = item.RepairPrice.Cost;
                item.LastModified = DateTime.UtcNow;
                item.Modified = true;
                // Add the new Appointment to the repository
                await appointmentRepository.UpdateAsync(item);
                return Result<Guid>.Success(item.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during updating Appointment");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
