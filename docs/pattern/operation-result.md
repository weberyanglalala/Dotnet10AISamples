# OperationResult 使用指南

## 概述

`OperationResult<T>` 是一個泛型類別，用於封裝服務層操作的結果。它提供了一致的回應格式，包含成功或失敗的狀態、資料、錯誤訊息和狀態碼。

## 類別結構

```csharp
public class OperationResult<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public string ErrorMessage { get; }
    public int Code { get; }

    // 私有建構函式
    private OperationResult(bool isSuccess, T data, string errorMessage, int code);

    // 靜態方法
    public static OperationResult<T> Success(T data, int statusCode = 200);
    public static OperationResult<T> Failure(string errorMessage = "Operation Failed.", int statusCode = 400);
}
```

## 屬性說明

- `IsSuccess`: 布林值，表示操作是否成功。
- `Data`: 泛型資料，成功時包含結果資料，失敗時為預設值。
- `ErrorMessage`: 字串，失敗時包含錯誤訊息。
- `Code`: 整數，HTTP 狀態碼或自訂狀態碼。

## 使用方法

### 成功回應

```csharp
// 在服務層中
public async Task<OperationResult<User>> CreateUserAsync(CreateUserDto dto)
{
    // 業務邏輯...
    var user = new User { /* ... */ };
    await _repository.CreateUserAsync(user);

    return OperationResult<User>.Success(user, 201);
}
```

### 失敗回應

```csharp
public async Task<OperationResult<User>> GetUserByIdAsync(string id)
{
    var user = await _repository.GetUserByIdAsync(id);
    if (user == null)
    {
        return OperationResult<User>.Failure("User not found", 404);
    }

    return OperationResult<User>.Success(user);
}
```

## 在專案中的應用

在 `Dotnet10AISamples.Api` 專案中，服務層方法應回傳 `OperationResult<T>` 以確保一致的服務回應格式。

控制器會檢查 `OperationResult.IsSuccess`，成功時將資料包裝成 `ApiResponse<T>` 回傳，失敗時使用 `Problem()` 方法。

### 範例服務

```csharp
public class UserService : IUserService
{
    public async Task<OperationResult<PaginatedResult<UserDto>>> GetPaginatedUsersAsync(UserQueryParameters parameters)
    {
        try
        {
            var (items, totalCount) = await _userRepository.GetPaginatedUsersAsync(parameters);
            var result = new PaginatedResult<UserDto>
            {
                Items = items.Select(u => u.ToDto()),
                PageNumber = parameters.Page,
                PageSize = parameters.PageSize,
                TotalCount = totalCount
            };

            return OperationResult<PaginatedResult<UserDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者列表時發生錯誤");
            return OperationResult<PaginatedResult<UserDto>>.Failure("取得使用者列表失敗", 500);
        }
    }
}
```

## 最佳實踐

1. 總是在服務層方法中回傳 `OperationResult<T>`
2. 使用適當的 HTTP 狀態碼
3. 提供有意義的錯誤訊息
4. 在控制器中檢查 `IsSuccess` 並適當處理

## 相關檔案

- `Dotnet10AISamples.Api/Common/OperationResult.cs`: OperationResult 類別定義
- `Dotnet10AISamples.Api/Services/`: 服務層範例
- `Dotnet10AISamples.Api/Controllers/`: 控制器如何使用 OperationResult
