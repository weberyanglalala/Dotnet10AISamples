using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.Entities;
using Dotnet10AISamples.Api.Repositories;

namespace Dotnet10AISamples.Api.Services;

/// <summary>
/// 角色服務實作
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<OperationResult<List<Role>>> GetUserRolesAsync(string userId)
    {
        try
        {
            var roles = await _roleRepository.GetUserRolesAsync(userId);
            return OperationResult<List<Role>>.Success(roles);
        }
        catch (Exception ex)
        {
            return OperationResult<List<Role>>.Failure($"取得使用者角色失敗: {ex.Message}", 500);
        }
    }

    public async Task<OperationResult<bool>> AssignRoleToUserAsync(string userId, string roleId, string assignedBy = null)
    {
        try
        {
            // 檢查使用者是否存在
            if (!await _roleRepository.UserExistsAsync(userId))
            {
                return OperationResult<bool>.Failure("使用者不存在", 404);
            }

            // 檢查角色是否存在
            if (!await _roleRepository.RoleExistsAsync(roleId))
            {
                return OperationResult<bool>.Failure("角色不存在", 404);
            }

            // 檢查是否已經有此角色
            if (await _roleRepository.UserHasRoleAsync(userId, roleId))
            {
                return OperationResult<bool>.Failure("使用者已經有此角色", 409);
            }

            // 指派角色
            var result = await _roleRepository.AssignRoleToUserAsync(userId, roleId, assignedBy);

            return OperationResult<bool>.Success(result > 0);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.Failure($"指派角色失敗: {ex.Message}", 500);
        }
    }

    public async Task<OperationResult<bool>> RemoveRoleFromUserAsync(string userId, string roleId)
    {
        try
        {
            var result = await _roleRepository.RemoveRoleFromUserAsync(userId, roleId);

            if (result == 0)
            {
                return OperationResult<bool>.Failure("使用者沒有此角色", 404);
            }

            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.Failure($"移除角色失敗: {ex.Message}", 500);
        }
    }

    public async Task<OperationResult<List<Role>>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return OperationResult<List<Role>>.Success(roles);
        }
        catch (Exception ex)
        {
            return OperationResult<List<Role>>.Failure($"取得角色列表失敗: {ex.Message}", 500);
        }
    }

    public async Task<OperationResult<Role>> GetRoleByNameAsync(string roleName)
    {
        try
        {
            var role = await _roleRepository.GetRoleByNameAsync(roleName);

            if (role == null)
            {
                return OperationResult<Role>.Failure("角色不存在", 404);
            }

            return OperationResult<Role>.Success(role);
        }
        catch (Exception ex)
        {
            return OperationResult<Role>.Failure($"取得角色失敗: {ex.Message}", 500);
        }
    }
}