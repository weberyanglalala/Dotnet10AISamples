using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.DTOs;
using Dotnet10AISamples.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet10AISamples.Api.Controllers;

/// <summary>
/// 認證控制器
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IRoleService _roleService;

    public AuthController(IAuthService authService, IRoleService roleService)
    {
        _authService = authService;
        _roleService = roleService;
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        IValidator<LoginRequestDto> validator)
    {
        await validator.ValidateAndThrowAsync(request);
        var result = await _authService.AuthenticateAsync(request.Email, request.Password);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        return Ok(new ApiResponse<AuthResponseDto> { Data = result.Data, Message = "Login successful", Code = 200 });
    }

    /// <summary>
    /// 取得目前登入使用者資訊
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserInfoDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userResult = await _authService.GetCurrentUserAsync();

        if (!userResult.IsSuccess)
        {
            return Problem(
                detail: userResult.ErrorMessage,
                statusCode: userResult.Code);
        }

        var user = userResult.Data;

        // 取得使用者角色
        var rolesResult = await _roleService.GetUserRolesAsync(user.Id);
        if (!rolesResult.IsSuccess)
        {
            return Problem(
                detail: "取得使用者角色失敗",
                statusCode: 500);
        }

        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            Roles = rolesResult.Data.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            }).ToList()
        };

        return Ok(new ApiResponse<UserInfoDto> { Data = userInfo, Message = "User info retrieved successfully", Code = 200 });
    }
}