namespace Domain.Entities;

public class User
{
    public User(string username, string email, string hashedPassword)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        HashedPassword = hashedPassword;
    }

    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string HashedPassword { get; private set; } = string.Empty;
    public ICollection<Workout> Workouts { get; private set; } = [];
}
