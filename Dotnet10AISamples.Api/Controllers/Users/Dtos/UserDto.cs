namespace Dotnet10AISamples.Api.Controllers.Users.Dtos;

public class UserDto
{
    /// <summary>
    /// 使用者的唯一識別碼
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 使用者的使用者名稱
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 使用者的電子郵件地址
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 指出使用者帳戶是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 使用者建立的日期和時間（UTC）
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 使用者最後更新的日期和時間（UTC）
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}