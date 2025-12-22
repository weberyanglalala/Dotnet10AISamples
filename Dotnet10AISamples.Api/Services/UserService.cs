using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.DTOs;
using Dotnet10AISamples.Api.Mappings;
using Dotnet10AISamples.Api.Repositories;

namespace Dotnet10AISamples.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<OperationResult<PaginatedResult<UserDto>>> GetAllUsersAsync(UserQueryParameters parameters)
    {
        try
        {
            var (items, totalCount) = await _userRepository.GetAllUsersAsync(parameters);

            var result = new PaginatedResult<UserDto>
            {
                Items = items.Select(u => u.ToDto()),
                Page = parameters.Page,
                PageSize = parameters.PageSize,
                TotalCount = totalCount
            };

            return OperationResult<PaginatedResult<UserDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者列表時發生錯誤");
            return OperationResult<PaginatedResult<UserDto>>.Failure("取得使用者列表失敗", 500);
        }
    }

    public async Task<OperationResult<UserDto>> GetUserByIdAsync(string id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return OperationResult<UserDto>.Failure("找不到使用者", 404);
            }

            return OperationResult<UserDto>.Success(user.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者 {Id} 時發生錯誤", id);
            return OperationResult<UserDto>.Failure("取得使用者失敗", 500);
        }
    }

    public async Task<OperationResult<UserDto>> CreateUserAsync(CreateUserDto dto)
    {
        try
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(dto.Username);
            if (existingUser != null)
            {
                return OperationResult<UserDto>.Failure("使用者名稱已存在", 409);
            }

            // Check if email already exists
            var existingEmail = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingEmail != null)
            {
                return OperationResult<UserDto>.Failure("電子郵件已存在", 409);
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create user entity
            var user = dto.ToEntity();
            user.PasswordHash = hashedPassword;

            var createdUser = await _userRepository.CreateUserAsync(user);

            _logger.LogInformation("已建立新使用者 {Username} ({Id})", createdUser.Username, createdUser.Id);

            return OperationResult<UserDto>.Success(createdUser.ToDto(), 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立使用者時發生錯誤");
            return OperationResult<UserDto>.Failure("建立使用者失敗", 500);
        }
    }

    public async Task<OperationResult<UserDto>> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return OperationResult<UserDto>.Failure("找不到使用者", 404);
            }

            // Update user
            dto.UpdateEntity(user);
            var updatedUser = await _userRepository.UpdateUserAsync(user);

            _logger.LogInformation("已更新使用者 {Username} ({Id})", updatedUser.Username, updatedUser.Id);

            return OperationResult<UserDto>.Success(updatedUser.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新使用者 {Id} 時發生錯誤", id);
            return OperationResult<UserDto>.Failure("更新使用者失敗", 500);
        }
    }

    public async Task<OperationResult<bool>> DeleteUserAsync(string id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return OperationResult<bool>.Failure("找不到使用者", 404);
            }

            await _userRepository.DeleteUserAsync(id);

            _logger.LogInformation("已刪除使用者 {Username} ({Id})", user.Username, user.Id);

            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除使用者 {Id} 時發生錯誤", id);
            return OperationResult<bool>.Failure("刪除使用者失敗", 500);
        }
    }
}