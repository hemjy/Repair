using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repair.Application.Common;
using Repair.Application.DTOs.Auth;
using Repair.Application.Features.Auth;

namespace Repair.Api.Controllers
{
    public class AuthController(ILogger<AuthController> logger) : BaseApiController
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(Result<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<LoginResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<LoginResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            try
            {
                if (command == null)
                {
                    logger.LogWarning("Null login command received");
                    return BadRequest(Result<LoginResponseDto>.Failure("Invalid login request"));
                }

                var result = await Mediator.Send(command);

                return result.Succeeded
                    ? Ok(result)
                    : BadRequest(result);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during login");
                return StatusCode(500, Result<LoginResponseDto>.Failure($"An unexpected error occurred: {ex.Message}"));
            }

        }

        [AllowAnonymous]
        [HttpPost("refreshToken")]
        [ProducesResponseType(typeof(Result<RefreshTokenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<RefreshTokenDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<RefreshTokenDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            try
            {
                if (command == null)
                {
                    logger.LogWarning("Null refresh token command received");
                    return BadRequest(Result<LoginResponseDto>.Failure("Invalid token request"));
                }

                var result = await Mediator.Send(command);

                return result.Succeeded
                    ? Ok(result)
                    : BadRequest(result);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during refresh token command");
                return StatusCode(500, Result<LoginResponseDto>.Failure($"An unexpected error occurred: {ex.Message}"));
            }

        }
    }
}
