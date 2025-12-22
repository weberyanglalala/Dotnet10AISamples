using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.Data;
using Dotnet10AISamples.Api.DTOs;
using Dotnet10AISamples.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet10AISamples.Api.Services;

/// <summary>
/// 角色服務實作
/// </summary>
public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<List<Role>>> GetUserRolesAsync(string userId)
    {
        try
        {
            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();

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
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return OperationResult<bool>.Failure("使用者不存在", 404);
            }

            // 檢查角色是否存在
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return OperationResult<bool>.Failure("角色不存在", 404);
            }

            // 檢查是否已經有此角色
            var existingRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (existingRole != null)
            {
                return OperationResult<bool>.Failure("使用者已經有此角色", 409);
            }

            // 建立新的使用者角色關聯
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return OperationResult<bool>.Success(true);
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
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
            {
                return OperationResult<bool>.Failure("使用者沒有此角色", 404);
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

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
            var roles = await _context.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();

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
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);

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