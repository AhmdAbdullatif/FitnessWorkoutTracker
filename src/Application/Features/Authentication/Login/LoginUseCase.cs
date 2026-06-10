using Application.Abstraction;
using Application.Exceptions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace Application.Features.Authentication.Login
{
    public class LoginUseCase(IUserRepository userRepository,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher
    )
    {
        public async Task<LoginResponse> ExecuteAsync(LoginCommand command)
        {
            User? user = await userRepository.GetByEmailAsync(command.Email);

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
