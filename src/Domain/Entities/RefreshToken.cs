namespace Domain.Entities;

public class RefreshToken
{
    public RefreshToken(string token, Guid userId)
    {
        Id = Guid.NewGuid();
        Token = token;
        ExpiresUtc = DateTime.UtcNow.AddDays(7);
        UserId = userId;
    }

    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresUtc { get; private set; }
    public DateTime? RevokedUtc { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; set; }

    public void Revoke()
    {
        if (RevokedUtc is not null)
            throw new InvalidOperationException("Refresh token was already revoked!");

        RevokedUtc = DateTime.UtcNow;
    }
}