using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.Controllers.Roles.Dtos;
using Dotnet10AISamples.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet10AISamples.Api.Controllers.Roles;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IAuthService _authService;

    public RolesController(IRoleService roleService, IAuthService authService)
    {
        _roleService = roleService;
        _authService = authService;
    }

    /// <summary>
    /// 取得所有角色
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<RoleDto>>), 200)]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _roleService.GetAllRolesAsync();

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        var roleDtos = result.Data.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description
        }).ToList();

        var paginatedResult = new PaginatedResult<RoleDto>
        {
            Items = roleDtos,
            PageNumber = 1,
            PageSize = roleDtos.Count,
            TotalCount = roleDtos.Count
            // TotalPages is computed automatically
        };

        return Ok(new ApiResponse<PaginatedResult<RoleDto>> { Data = paginatedResult, Message = "Roles retrieved successfully", Code = 200 });
    }

    /// <summary>
    /// 指派角色給使用者
    /// </summary>
    [HttpPost("users/{userId}/roles")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(201)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AssignRole(
        string userId, [FromBody] AssignRoleDto request,
        IValidator<AssignRoleDto> validator
        )
    {
        await validator.ValidateAndThrowAsync(request);
        // 取得目前使用者作為指派者
        var currentUserResult = await _authService.GetCurrentUserAsync();
        if (!currentUserResult.IsSuccess)
        {
            return Problem(
                detail: "無法取得目前使用者資訊",
                statusCode: 401);
        }

        var result = await _roleService.AssignRoleToUserAsync(
            userId,
            request.RoleId,
            currentUserResult.Data.Id);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        return CreatedAtAction(
            nameof(GetRoles),
            new { },
            new { message = "角色已成功指派" });
    }

    /// <summary>
    /// 移除使用者的角色
    /// </summary>
    [HttpDelete("users/{userId}/roles/{roleId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveRole(string userId, string roleId)
    {
        var result = await _roleService.RemoveRoleFromUserAsync(userId, roleId);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        return NoContent();
    }
}