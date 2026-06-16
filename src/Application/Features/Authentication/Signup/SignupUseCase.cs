using System.Security.Cryptography.X509Certificates;
using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications;
using Domain.Entities;

namespace Application.Features.Authentication.Signup;

public class SignupUseCase(IRepository<User> repository,
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher
) : ISignupUseCase

{
    public async Task<SignupResult> ExecuteAsync(SignupCommand command)
    {
        var spec = new GetUserByEmailReadonlySpec(command.Email);
        User? user = await repository.FirstOrDefaultAsync(spec);
        if (user is not null)
            throw new EmailConflictException();

        var hashedPassword = passwordHasher.HashPassword(command.Password);
        user = new User(command.Username, command.Email, hashedPassword);

        await repository.AddAsync(user);

        var token = jwtProvider.Create(user);

        return new SignupResult()
        {
            Token = token
        };
    }
}
