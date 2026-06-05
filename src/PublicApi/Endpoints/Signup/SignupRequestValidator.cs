using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using FluentValidation;

namespace PublicApi.Endpoints.Signup;

public class SignupRequestValidator : Validator<SignupRequest>
{
    public SignupRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name is too short!")
            .MaximumLength(30).WithMessage("Name is too long!");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email")
            .MaximumLength(256).WithMessage("Email is too long!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password is too short!")
            .MaximumLength(256).WithMessage("Password is too long!");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Confirm password does not match the password!");
    }
}
