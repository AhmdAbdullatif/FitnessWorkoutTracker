namespace Infrastructure.Services.Authentication;

public class JwtOptions
{
    public int LifeTime { get; set; }
    public string SigningKey { get; set; } = string.Empty;

}
