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
        IPasswordHasher passwordHasher
    ) : ILoginUseCase
    {
        public async Task<LoginResponse> ExecuteAsync(LoginCommand command)
        {
            var spec = new GetUserByEmailReadonlySpec(command.Email);
            User? user = await readRepository.FirstOrDefaultAsync(spec);

            if (user is null)
                throw new InvalidUserCredentialsException();

            var isPasswordCorrect = passwordHasher.VerifyPassword(user.HashedPassword, command.Password);
            if (!isPasswordCorrect)
                throw new InvalidUserCredentialsException();

            var token = jwtProvider.Create(user);

            return new LoginResponse()
            {
                Token = token,
            };
        }
    }
}
