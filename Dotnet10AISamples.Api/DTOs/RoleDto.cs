namespace Dotnet10AISamples.Api.DTOs;

/// <summary>
/// 角色 DTO
/// </summary>
public class RoleDto
{
    /// <summary>
    /// 角色 ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 角色名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; }
}