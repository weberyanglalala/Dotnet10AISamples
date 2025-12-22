using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.DTOs;

namespace Dotnet10AISamples.Api.Services;

public interface IUserService
{
    Task<OperationResult<PaginatedResult<UserDto>>> GetAllUsersAsync(UserQueryParameters parameters);
    Task<OperationResult<UserDto>> GetUserByIdAsync(string id);
    Task<OperationResult<UserDto>> CreateUserAsync(CreateUserDto dto);
    Task<OperationResult<UserDto>> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<OperationResult<bool>> DeleteUserAsync(string id);
}