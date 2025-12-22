namespace Dotnet10AISamples.Api.Entities;

/// <summary>
/// 使用者角色關聯實體（多對多關係）
/// </summary>
public class UserRole
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 角色 ID
    /// </summary>
    public string RoleId { get; set; } = string.Empty;

    /// <summary>
    /// 角色指派的日期和時間（UTC）
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 指派此角色的使用者 ID（可為空）
    /// </summary>
    public string AssignedBy { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Role Role { get; set; }
    public User AssignedByUser { get; set; }
}