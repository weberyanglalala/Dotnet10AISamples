using Dotnet10AISamples.Api.Controllers.Users.Dtos;
using FluentValidation;

namespace Dotnet10AISamples.Api.Validators;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        // IsActive is optional, no specific validation rules needed
    }
}