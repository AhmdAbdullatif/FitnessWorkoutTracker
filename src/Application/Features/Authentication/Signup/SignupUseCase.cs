using System.Security.Cryptography.X509Certificates;
using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications;
using Domain.Entities;

namespace Application.Features.Authentication.Signup;

public class SignupUseCase(IRepository<User> userRepository,
    IRepository<RefreshToken> refreshRepository,
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher,
    IAppLogger<SignupUseCase> logger
) : ISignupUseCase

{
    public async Task<AuthenticateResponse> ExecuteAsync(SignupCommand command)
    {
        var spec = new GetUserByEmailReadonlySpec(command.Email);
        User? user = await userRepository.FirstOrDefaultAsync(spec);
        if (user is not null)
        {
            logger.LogWarning("Failed signup attempt for email: {Email}. Reason: Email already exists.",
                command.Email);
            throw new EmailConflictException();
        }

        var hashedPassword = passwordHasher.HashPassword(command.Password);
        user = new User(command.Username, command.Email, hashedPassword);

        await userRepository.AddAsync(user);

        var accessToken = jwtProvider.CreateAccessToken(user.Id, user.Email);
        var refreshToken = new RefreshToken(jwtProvider.CreateRefreshToken(), user.Id);

        await refreshRepository.AddAsync(refreshToken);

        logger.LogInformation("User signed up successfully. UserId: {UserId}",
            user.Id);

        return new AuthenticateResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}
