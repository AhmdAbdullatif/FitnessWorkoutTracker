using System.Security.Cryptography.X509Certificates;
using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications;
using Domain.Entities;

namespace Application.Features.Authentication.Signup;

public class SignupUseCase(IRepository<User> repository,
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher,
    IAppLogger<SignupUseCase> logger
) : ISignupUseCase

{
    public async Task<SignupResult> ExecuteAsync(SignupCommand command)
    {
        var spec = new GetUserByEmailReadonlySpec(command.Email);
        User? user = await repository.FirstOrDefaultAsync(spec);
        if (user is not null)
        {
            logger.LogWarning("Failed signup attempt for email: {Email}. Reason: Email already exists.",
                command.Email);
            throw new EmailConflictException();
        }

        var hashedPassword = passwordHasher.HashPassword(command.Password);
        user = new User(command.Username, command.Email, hashedPassword);

        await repository.AddAsync(user);

        var token = jwtProvider.Create(user.Id, user.Email);

        logger.LogInformation("User signed up successfully. UserId: {UserId}",
            user.Id);

        return new SignupResult()
        {
            Token = token
        };
    }
}
