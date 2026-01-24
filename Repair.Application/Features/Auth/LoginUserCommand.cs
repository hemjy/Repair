using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Auth;
using Repair.Application.Interfaces.Auth;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Application.Features.Auth
{
    public class LoginUserCommand : IRequest<Result<LoginResponseDto>>
    {

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string? Password { get; set; }
    }

    public class LoginUserCommandHandler(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            ILogger<LoginUserCommandHandler> logger,
            IJwtTokenGenerator jwtTokenGenerator,
            IGenericRepositoryAsync<RefreshToken> refreshTokenRepo) : IRequestHandler<LoginUserCommand, Result<LoginResponseDto>>
    {

        public async Task<Result<LoginResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling login request for email: {Email}", request.Email);

                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(request);

                bool isValid = Validator.TryValidateObject(request, context, validationResults, true);


                if (!isValid)
                {
                    logger.LogWarning("Validation failed for /login request: {ValidationResults}", validationResults);
                    return Result<LoginResponseDto>.Failure("failed", validationResults);
                }


                var user = await userManager.FindByNameAsync(request.Email.Trim().ToLower());

                if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
                {
                    return Result<LoginResponseDto>.Failure("Invalid credentials");
                }

                var roles = await userManager.GetRolesAsync(user);
                logger.LogInformation("User roles: {Roles}", string.Join(", ", roles));

                var (accessToken, refreshToken, refreshTokenExp) = jwtTokenGenerator
                                                    .GenerateJwtTokenInfo(user.Id, user.UserName, (List<string>)roles);

                var refreshTokenEntity = RefreshToken.Create(user.Id, refreshToken, refreshTokenExp);

                await refreshTokenRepo.AddAsync(refreshTokenEntity);

                var loginResponseDto = new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = $"{user.LastName} {user.FirstName}",

                };

                return Result<LoginResponseDto>.Success(loginResponseDto, "Login successful");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during user login");
                return Result<LoginResponseDto>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
