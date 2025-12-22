namespace Dotnet10AISamples.Api.Controllers.Users.Dtos;

public class CreateUserDto
{
    /// <summary>
    /// 使用者的使用者名稱
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 使用者的電子郵件地址
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者的密碼
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 指出使用者帳戶是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;
}