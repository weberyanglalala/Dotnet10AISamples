using Dotnet10AISamples.Api.DTOs;
using Dotnet10AISamples.Api.Entities;

namespace Dotnet10AISamples.Api.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static User ToEntity(this CreateUserDto dto)
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = dto.Password, // Will be hashed in service layer
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this UpdateUserDto dto, User user)
    {
        if (dto.IsActive.HasValue)
        {
            user.IsActive = dto.IsActive.Value;
        }
        user.UpdatedAt = DateTime.UtcNow;
    }
}