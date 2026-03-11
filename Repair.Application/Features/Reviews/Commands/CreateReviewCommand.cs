using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Appointments;
using Repair.Application.DTOs.EmailSetting;
using Repair.Application.DTOs.Reviews;
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
    public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.OrderNumber).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.Comment).NotEmpty().WithMessage("{PropertyName} field is required");
            RuleFor(x => x.Rating).LessThanOrEqualTo(5).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} field is required");

        }


    }
    public class CreateReviewCommand : CreateReviewDto, IRequest<Result<Guid>>
    {
    }

    public class CreateReviewCommandHandler(ILogger<CreateReviewCommandHandler> logger,
            IGenericRepositoryAsync<Appointment> appointmentRepository,
            IGenericRepositoryAsync<Feedback> feedbackRepository) : IRequestHandler<CreateReviewCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling creating Appointment");

               var appointment = await appointmentRepository.GetByAsync(x => request.OrderNumber.Trim().ToLower() == x.OrderNumber.Trim().ToLower() && !x.IsDeleted);
                if (appointment is null) return Result<Guid>.Failure("Appointment Does not exist.");

                var item = Feedback.Create(appointment.Id, request.Rating, request.Comment);

                await feedbackRepository.AddAsync(item);

                return Result<Guid>.Success(item.Id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during creating Review");
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
