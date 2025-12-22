# ApiResponse 使用指南

## 概述

`ApiResponse<T>` 是一個泛型類別，用於封裝 API 呈現層的回應。它提供了一致的 JSON 回應格式，包含資料、訊息和狀態碼。

## 類別結構

```csharp
public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public int Code { get; set; }
}
```

## 屬性說明

- `Data`: 泛型資料，包含結果資料。
- `Message`: 字串，成功或失敗的訊息。
- `Code`: 整數，HTTP 狀態碼。

## 使用方法

### 成功回應

```csharp
// 在控制器中
[HttpGet]
public IActionResult GetWeather()
{
    var forecasts = GetWeatherForecasts();

    return Ok(new ApiResponse<IEnumerable<WeatherForecast>>
    {
        Data = forecasts,
        Message = "Weather forecast retrieved successfully",
        Code = 200
    });
}
```

### 分頁回應

```csharp
[HttpGet]
public IActionResult GetUsers(int page = 1, int pageSize = 10)
{
    var result = _userService.GetPaginatedUsersAsync(new UserQueryParameters { Page = page, PageSize = pageSize });

    if (!result.IsSuccess)
    {
        return Problem(detail: result.ErrorMessage, statusCode: result.Code);
    }

    return Ok(new ApiResponse<PaginatedResult<UserDto>>
    {
        Data = result.Data,
        Message = "Users retrieved successfully",
        Code = 200
    });
}
```

### 建立資源

```csharp
[HttpPost]
public IActionResult CreateUser(CreateUserDto dto)
{
    var result = _userService.CreateUserAsync(dto);

    if (!result.IsSuccess)
    {
        return Problem(detail: result.ErrorMessage, statusCode: result.Code);
    }

    return CreatedAtAction(nameof(GetUser), new { id = result.Data.Id }, new ApiResponse<UserDto>
    {
        Data = result.Data,
        Message = "User created successfully",
        Code = 201
    });
}
```

### 失敗回應

失敗時仍使用 `Problem()` 方法回傳標準的 ProblemDetails 回應：

```csharp
[HttpGet]
public IActionResult GetUser(string id)
{
    var result = _userService.GetUserByIdAsync(id);

    if (!result.IsSuccess)
    {
        return Problem(detail: result.ErrorMessage, statusCode: result.Code);
    }

    return Ok(new ApiResponse<UserDto>
    {
        Data = result.Data,
        Message = "User retrieved successfully",
        Code = 200
    });
}
```

## 在專案中的應用

在 `Dotnet10AISamples.Api` 專案中，所有控制器都應繼承 `ControllerBase` 並回傳 `ApiResponse<T>` 以確保一致的 API 回應格式。

錯誤處理應整合到已註冊的 `ProblemDetails` 管道中。對於驗證失敗，請使用 `ValidationException`。

### 範例控制器

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        var result = _userService.GetPaginatedUsersAsync(parameters);

        if (!result.IsSuccess)
        {
            return Problem(detail: result.ErrorMessage, statusCode: result.Code);
        }

        return Ok(new ApiResponse<PaginatedResult<UserDto>>
        {
            Data = result.Data,
            Message = "Users retrieved successfully",
            Code = 200
        });
    }
}
```

## 最佳實踐

1. 總是在控制器方法中檢查 `OperationResult.IsSuccess`（服務層仍使用 OperationResult）
2. 成功時回傳 `ApiResponse<T>` 包裝的資料
3. 失敗時使用 `Problem()` 方法回傳標準的 ProblemDetails 回應
4. 使用適當的 HTTP 狀態碼和訊息
5. 提供有意義的成功訊息
6. 對於驗證錯誤，使用專門的驗證異常處理器

## 相關檔案

- `Dotnet10AISamples.Api/Common/ApiResponse.cs`: ApiResponse 類別定義
- `Dotnet10AISamples.Api/Common/OperationResult.cs`: 服務層使用的 OperationResult
- `Dotnet10AISamples.Api/Middlewares/`: 異常處理中介軟體
- `Dotnet10AISamples.Api/Controllers/`: 控制器範例
