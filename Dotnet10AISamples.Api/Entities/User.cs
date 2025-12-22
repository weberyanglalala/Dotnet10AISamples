namespace Dotnet10AISamples.Api.Entities;

public class User
{
    /// <summary>
    /// 使用者的唯一識別碼（GUID 字串）
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 使用者的使用者名稱（必須唯一）
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 使用者的電子郵件地址（必須唯一）
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者的密碼雜湊
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 指出使用者帳戶是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 使用者建立的日期和時間（UTC）
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 使用者最後更新的日期和時間（UTC）
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}