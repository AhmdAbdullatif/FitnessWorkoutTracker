namespace Application.Abstraction;

public interface IAppLogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogDebug(string message, params object[] args);
}
