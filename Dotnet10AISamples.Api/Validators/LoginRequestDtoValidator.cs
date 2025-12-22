using Dotnet10AISamples.Api.DTOs;
using FluentValidation;

namespace Dotnet10AISamples.Api.Validators;

/// <summary>
/// LoginRequestDto 驗證器
/// </summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("電子郵件為必填")
            .EmailAddress().WithMessage("電子郵件格式不正確")
            .MaximumLength(256).WithMessage("電子郵件長度不能超過 256 個字元");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密碼為必填")
            .MinimumLength(6).WithMessage("密碼長度至少 6 個字元");
    }
}