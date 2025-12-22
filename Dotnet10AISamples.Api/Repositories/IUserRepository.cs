using Dotnet10AISamples.Api.Entities;
using Dotnet10AISamples.Api.Mappings;

namespace Dotnet10AISamples.Api.Repositories;

public interface IUserRepository
{
    Task<(IEnumerable<User> Items, int TotalCount)> GetPaginatedUsersAsync(UserQueryParameters parameters);
    Task<User> GetUserByIdAsync(string id);
    Task<User> GetUserByUsernameAsync(string username);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetActiveUserByIdAsync(string id);
    Task<User> GetActiveUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(string id);
    Task<bool> UserExistsAsync(string id);
}