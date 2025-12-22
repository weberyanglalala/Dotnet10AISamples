using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.DTOs;
using Dotnet10AISamples.Api.Entities;

namespace Dotnet10AISamples.Api.Services;

/// <summary>
/// 認證服務介面
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 驗證使用者並產生 JWT token
    /// </summary>
    /// <param name="email">使用者電子郵件</param>
    /// <param name="password">使用者密碼</param>
    /// <returns>認證結果</returns>
    Task<OperationResult<AuthResponseDto>> AuthenticateAsync(string email, string password);

    /// <summary>
    /// 產生 JWT token
    /// </summary>
    /// <param name="user">使用者實體</param>
    /// <returns>JWT token 字串</returns>
    string GenerateJwtToken(User user);

    /// <summary>
    /// 取得目前登入使用者
    /// </summary>
    /// <returns>目前使用者</returns>
    Task<OperationResult<User>> GetCurrentUserAsync();
}