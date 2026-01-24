

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repair.Application.Common;
using Repair.Application.DTOs.Auth;
using Repair.Application.Interfaces.Auth;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using System.ComponentModel.DataAnnotations;

namespace Repair.Application.Features.Auth
{
    public class RefreshTokenCommand : IRequest<Result<RefreshTokenDto>>
    {
        [Required]
        public string? RefreshToken { get; set; }
    }

    internal class RefreshTokenCommandHandler(
            UserManager<User> userManager,
            IConfiguration configuration,
            IJwtTokenGenerator jwtTokenGenerator,
            IGenericRepositoryAsync<RefreshToken> refreshTokenRepo,
            ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenDto>>
    {


        public async Task<Result<RefreshTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling refresh token request");

                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(request);

                bool isValid = Validator.TryValidateObject(request, context, validationResults, true);


                if (!isValid)
                {
                    logger.LogWarning("Validation failed for refresh token: {ValidationResults}", validationResults);
                    return Result<RefreshTokenDto>.Failure("failed", validationResults);
                }
                var tokenEntity = await refreshTokenRepo.GetByAsync(t => t.Token == request.RefreshToken && !t.IsRevoked);

                if (tokenEntity == null || tokenEntity.ExpirationDate < DateTime.UtcNow)
                {
                    return Result<RefreshTokenDto>.Failure("Invalid or expired token.");
                }

                var user = await userManager.FindByIdAsync(tokenEntity.UserId.ToString());

                var roles = await userManager.GetRolesAsync(user);
                logger.LogInformation("User roles: {Roles}", string.Join(", ", roles));

                var (accessToken, refreshToken, refreshTokenExp) = jwtTokenGenerator
                                                                    .GenerateJwtTokenInfo(user.Id, user.UserName, (List<string>)roles);
                // Revoke old refresh token and store the new one
                tokenEntity.IsRevoked = true;

                var refreshTokenEntity = RefreshToken.Create(user.Id, refreshToken, refreshTokenExp);

                await refreshTokenRepo.AddAsync(refreshTokenEntity, false);
                await refreshTokenRepo.UpdateAsync(tokenEntity, false);

                var loginResponseDto = new RefreshTokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };

                await refreshTokenRepo.SaveChangesAsync();

                return Result<RefreshTokenDto>.Success(loginResponseDto, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during refresh token");
                return Result<RefreshTokenDto>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
