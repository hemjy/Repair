using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Appointments;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.Features.Appointments.Commands
{
    public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentValidator()
        {
            RuleFor(x => x.RepairPriceId).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.CustomerPhoneNumber).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.CustomerEmail).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.CustomerLastname).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.CustomerFirstName).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.AppointmentDay).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.AppointmentTime).NotEmpty().WithMessage("{PropertyName} field is required");

        }


    }
    public class CreateAppointmentCommand : CreateAppointmentDto, IRequest<Result<Guid>>
    {
    }

    public class CreateAppointmentCommandHandler(ILogger<CreateAppointmentCommandHandler> logger,
            IGenericRepositoryAsync<Appointment> appointmentRepository,
            IGenericRepositoryAsync<RepairPrice> repairPriceRepository,
            IFileStorageService fileStorage) : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating Appointment");

               var repairPriceExist = await repairPriceRepository.IsUniqueAsync(x => x.Id == request.RepairPriceId && !x.IsDeleted);
                if (!repairPriceExist) return Result<Guid>.Failure("Repair Part Does not exist.");

                if(!TimeOnly.TryParse(request.AppointmentTime, out var appointmentTime)) return Result<Guid>.Failure("Invalid Appointment Time");
                // Create a new Appointment 
                var newAppointment = Appointment
                    .Create(repairPriceId: request.RepairPriceId, comment: request.Comment, customerEmail: request.CustomerEmail, customerPhoneNumber: request.CustomerPhoneNumber,
                    customerLastname: request.CustomerLastname, customerFirstName: request.CustomerFirstName, CustomerCity: request.CustomerCity, CustomerAddress: request.CustomerAddress,
                    CustomerState: request.CustomerState, appointmentDay: request.AppointmentDay, appointmentTime: appointmentTime);

                // Add the new Appointment to the repository
                await appointmentRepository.AddAsync(newAppointment);
                return Result<Guid>.Success(newAppointment.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during creating Appointment");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
