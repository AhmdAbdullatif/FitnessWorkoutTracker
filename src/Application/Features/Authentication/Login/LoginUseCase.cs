using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace Application.Features.Authentication.Login
{
    public class LoginUseCase(IReadRepository<User> readRepository,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher,
        IAppLogger<LoginUseCase> logger
    ) : ILoginUseCase
    {
        public async Task<LoginResponse> ExecuteAsync(LoginCommand command)
        {
            var spec = new GetUserByEmailReadonlySpec(command.Email);
            User? user = await readRepository.FirstOrDefaultAsync(spec);

            if (user is null)
            {
                logger.LogWarning("Failed login attempt for email: {Email}. Reason: User not found.",
                command.Email);

                throw new InvalidUserCredentialsException();
            }

            var isPasswordCorrect = passwordHasher.VerifyPassword(user.HashedPassword, command.Password);
            if (!isPasswordCorrect)
            {
                logger.LogWarning("Failed login attempt for email: {Email}. Reason: Invalid password.",
                command.Email);

                throw new InvalidUserCredentialsException();
            }

            var token = jwtProvider.Create(user);

            logger.LogInformation("User logged in successfully. UserId: {UserId}",
            user.Id);

            return new LoginResponse()
            {
                Token = token,
            };
        }
    }
}
