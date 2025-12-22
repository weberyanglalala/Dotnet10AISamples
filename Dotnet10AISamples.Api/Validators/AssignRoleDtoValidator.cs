using Dotnet10AISamples.Api.DTOs;
using FluentValidation;

namespace Dotnet10AISamples.Api.Validators;

/// <summary>
/// AssignRoleDto 驗證器
/// </summary>
public class AssignRoleDtoValidator : AbstractValidator<AssignRoleDto>
{
    public AssignRoleDtoValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色 ID 為必填");
    }
}