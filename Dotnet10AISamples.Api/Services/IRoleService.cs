using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.DTOs;
using Dotnet10AISamples.Api.Entities;

namespace Dotnet10AISamples.Api.Services;

/// <summary>
/// 角色服務介面
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 取得使用者角色
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>使用者角色列表</returns>
    Task<OperationResult<List<Role>>> GetUserRolesAsync(string userId);

    /// <summary>
    /// 指派角色給使用者
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <param name="roleId">角色 ID</param>
    /// <param name="assignedBy">指派者使用者 ID</param>
    /// <returns>操作結果</returns>
    Task<OperationResult<bool>> AssignRoleToUserAsync(string userId, string roleId, string assignedBy = null);

    /// <summary>
    /// 移除使用者的角色
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <param name="roleId">角色 ID</param>
    /// <returns>操作結果</returns>
    Task<OperationResult<bool>> RemoveRoleFromUserAsync(string userId, string roleId);

    /// <summary>
    /// 取得所有可用角色
    /// </summary>
    /// <returns>角色列表</returns>
    Task<OperationResult<List<Role>>> GetAllRolesAsync();

    /// <summary>
    /// 根據名稱取得角色
    /// </summary>
    /// <param name="roleName">角色名稱</param>
    /// <returns>角色實體</returns>
    Task<OperationResult<Role>> GetRoleByNameAsync(string roleName);
}