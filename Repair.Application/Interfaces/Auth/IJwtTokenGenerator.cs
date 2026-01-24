

namespace Repair.Application.Interfaces.Auth
{
    public interface IJwtTokenGenerator
    {
        (string token, string refreshToken, DateTime refreshTokenExp) GenerateJwtTokenInfo(Guid userId, string username, List<string> roles);
        string GenerateRefreshToken();
    }
}
