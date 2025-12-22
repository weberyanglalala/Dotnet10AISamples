namespace Dotnet10AISamples.Api.Controllers.Auth.Dtos;

/// <summary>
/// 登入請求 DTO
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// 使用者電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼
    /// </summary>
    public string Password { get; set; } = string.Empty;
}