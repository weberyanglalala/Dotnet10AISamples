using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.Data;
using Dotnet10AISamples.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dotnet10AISamples.Api.Controllers.Auth.Dtos;
using Dotnet10AISamples.Api.Controllers.Roles.Dtos;
using Dotnet10AISamples.Api.Controllers.Users.Dtos;
using Dotnet10AISamples.Api.Repositories;
using Dotnet10AISamples.Api.Services;

namespace Dotnet10AISamples.Api.Services;

/// <summary>
/// 認證服務實作
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public AuthService(
        ApplicationDbContext context,
        IOptions<JwtSettings> jwtSettings,
        IHttpContextAccessor httpContextAccessor,
        IRoleRepository roleRepository,
        IUserRepository userRepository)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _httpContextAccessor = httpContextAccessor;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<OperationResult<AuthResponseDto>> AuthenticateAsync(string email, string password)
    {
        try
        {
            // 尋找使用者
            var user = await _userRepository.GetActiveUserByEmailAsync(email);

            if (user == null)
            {
                return OperationResult<AuthResponseDto>.Failure("無效的憑證", 401);
            }

            // 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return OperationResult<AuthResponseDto>.Failure("無效的憑證", 401);
            }

            // 取得使用者角色
            var roles = await _roleRepository.GetUserRolesAsync(user.Id);

            // 產生 JWT token
            var token = GenerateJwtToken(user, roles);

            // 建立回應
            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToList()
            };

            var response = new AuthResponseDto
            {
                Token = token,
                User = userInfo,
                ExpiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours)
            };

            return OperationResult<AuthResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return OperationResult<AuthResponseDto>.Failure($"認證失敗: {ex.Message}", 500);
        }
    }

    public string GenerateJwtToken(User user, List<Role> roles = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        // 取得使用者角色並加入 claims
        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<OperationResult<User>> GetCurrentUserAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return OperationResult<User>.Failure("使用者未認證", 401);
            }

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return OperationResult<User>.Failure("無效的認證資訊", 401);
            }

            var user = await _userRepository.GetActiveUserByIdAsync(userId);

            if (user == null)
            {
                return OperationResult<User>.Failure("使用者不存在", 404);
            }

            return OperationResult<User>.Success(user);
        }
        catch (Exception ex)
        {
            return OperationResult<User>.Failure($"取得目前使用者失敗: {ex.Message}", 500);
        }
    }
}