using Dotnet10AISamples.Api.Common;
using Dotnet10AISamples.Api.Controllers.Users.Dtos;
using Dotnet10AISamples.Api.Mappings;
using Dotnet10AISamples.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet10AISamples.Api.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 取得所有使用者列表
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserDto>>), 200)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isActive = false,
        [FromQuery] bool isActiveFilterEnabled = false,
        [FromQuery] string search = "")
    {
        var parameters = new UserQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            IsActive = isActive,
            IsActiveFilterEnabled = isActiveFilterEnabled,
            Search = search
        };

        var result = await _userService.GetPaginatedUsersAsync(parameters);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        return Ok(new ApiResponse<PaginatedResult<UserDto>> { Data = result.Data, Message = "Users retrieved successfully", Code = 200 });
    }

    /// <summary>
    /// 根據 ID 取得單一使用者
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser(string id)
    {
        var result = await _userService.GetUserByIdAsync(id);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        return Ok(new ApiResponse<UserDto> { Data = result.Data, Message = "User retrieved successfully", Code = 200 });
    }

    /// <summary>
    /// 建立新使用者
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserDto dto,
        IValidator<CreateUserDto> validator
    )
    {
        await validator.ValidateAndThrowAsync(dto);
        var result = await _userService.CreateUserAsync(dto);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        return CreatedAtAction(nameof(GetUser), new { id = result.Data.Id }, new ApiResponse<UserDto> { Data = result.Data, Message = "User created successfully", Code = 201 });
    }

    /// <summary>
    /// 更新現有使用者
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateUser(
        string id,
        [FromBody] UpdateUserDto dto,
        IValidator<UpdateUserDto> validator
    )
    {
        await validator.ValidateAndThrowAsync(dto);
        var result = await _userService.UpdateUserAsync(id, dto);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        return Ok(new ApiResponse<UserDto> { Data = result.Data, Message = "User updated successfully", Code = 200 });
    }

    /// <summary>
    /// 刪除使用者（軟刪除）
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (!result.IsSuccess)
        {
            return Problem(
                detail: result.ErrorMessage,
                statusCode: result.Code);
        }

        return NoContent();
    }
}