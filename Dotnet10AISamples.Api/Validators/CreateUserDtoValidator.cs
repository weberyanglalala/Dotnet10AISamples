using Dotnet10AISamples.Api.Controllers.Users.Dtos;
using FluentValidation;

namespace Dotnet10AISamples.Api.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("使用者名稱為必填")
            .Length(3, 50).WithMessage("使用者名稱長度必須在 3 到 50 字元之間")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("使用者名稱只能包含字母、數字和底線");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("電子郵件為必填")
            .EmailAddress().WithMessage("電子郵件格式不正確");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密碼為必填")
            .MinimumLength(8).WithMessage("密碼長度至少 8 字元")
            .Matches(@"[a-z]").WithMessage("密碼必須包含小寫字母")
            .Matches(@"[A-Z]").WithMessage("密碼必須包含大寫字母")
            .Matches(@"[0-9]").WithMessage("密碼必須包含數字");
    }
}