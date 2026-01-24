

namespace Repair.Domain
{
    public class RefreshToken : EntityBase
    {
        private RefreshToken() { }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool IsRevoked { get; set; }

        public static RefreshToken Create(Guid UserId, string refreshToken, DateTime expiration) => new()
        {
            UserId = UserId,
            ExpirationDate = expiration,
            Token = refreshToken
        };
    }
}
