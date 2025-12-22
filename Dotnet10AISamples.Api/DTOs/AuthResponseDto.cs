namespace Dotnet10AISamples.Api.DTOs;

/// <summary>
/// 認證回應 DTO
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 使用者資訊
    /// </summary>
    public UserInfoDto User { get; set; } = new();

    /// <summary>
    /// Token 到期時間
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}