using Dotnet10AISamples.Api.Entities;

namespace Dotnet10AISamples.Api.Repositories;

/// <summary>
/// 角色資料存取介面
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// 取得使用者角色列表
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>角色列表</returns>
    Task<List<Role>> GetUserRolesAsync(string userId);

    /// <summary>
    /// 檢查使用者是否存在
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>是否存在</returns>
    Task<bool> UserExistsAsync(string userId);

    /// <summary>
    /// 檢查角色是否存在
    /// </summary>
    /// <param name="roleId">角色 ID</param>
    /// <returns>是否存在</returns>
    Task<bool> RoleExistsAsync(string roleId);

    /// <summary>
    /// 檢查使用者是否已經有指定角色
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <param name="roleId">角色 ID</param>
    /// <returns>是否已有角色</returns>
    Task<bool> UserHasRoleAsync(string userId, string roleId);

    /// <summary>
    /// 指派角色給使用者
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <param name="roleId">角色 ID</param>
    /// <param name="assignedBy">指派者 ID</param>
    /// <returns>受影響的行數</returns>
    Task<int> AssignRoleToUserAsync(string userId, string roleId, string assignedBy);

    /// <summary>
    /// 移除使用者的角色
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <param name="roleId">角色 ID</param>
    /// <returns>受影響的行數</returns>
    Task<int> RemoveRoleFromUserAsync(string userId, string roleId);

    /// <summary>
    /// 取得所有角色
    /// </summary>
    /// <returns>角色列表</returns>
    Task<List<Role>> GetAllRolesAsync();

    /// <summary>
    /// 根據名稱取得角色
    /// </summary>
    /// <param name="roleName">角色名稱</param>
    /// <returns>角色實體</returns>
    Task<Role> GetRoleByNameAsync(string roleName);
}