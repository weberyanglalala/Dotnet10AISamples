using Dotnet10AISamples.Api.Controllers.Roles.Dtos;

namespace Dotnet10AISamples.Api.Controllers.Users.Dtos;

/// <summary>
/// 使用者資訊 DTO（用於 /me 端點）
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 使用者電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者角色列表
    /// </summary>
    public List<RoleDto> Roles { get; set; } = new();
}