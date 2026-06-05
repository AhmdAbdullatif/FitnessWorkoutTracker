using Application.Abstraction;
using Application.Exceptions;
using Domain.Entities;

namespace Application.Features.Authentication.Signup;

public class SignupUseCase(IUserRepository userRepository,
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher
)

{
    public async Task<SignupResult> ExecuteAsync(SignupCommand command)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email);
        if (user is not null)
            throw new EmailConflict();

        var hashedPassword = passwordHasher.HashPassword(command.Password);
        user = new User(command.Username, command.Email, hashedPassword);

        await userRepository.AddUserAsync(user);

        var token = jwtProvider.Create(user);

        return new SignupResult()
        {
            Token = token
        };
    }
}
