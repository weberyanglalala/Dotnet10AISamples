namespace Dotnet10AISamples.Api.Entities;

/// <summary>
/// 角色實體
/// </summary>
public class Role
{
    /// <summary>
    /// 角色的唯一識別碼（GUID 字串）
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 角色名稱（必須唯一）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 角色建立的日期和時間（UTC）
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 角色最後更新的日期和時間（UTC）
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}