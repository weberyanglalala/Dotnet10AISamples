# OperationResult 使用指南

## 概述

`OperationResult<T>` 是一個泛型類別，用於封裝 API 操作的結果。它提供了一致的回應格式，包含成功或失敗的狀態、資料、錯誤訊息和狀態碼。

## 類別結構

```csharp
public class OperationResult<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public string ErrorMessage { get; }
    public string Code { get; }

    // 私有建構函式
    private OperationResult(bool isSuccess, T data, string errorMessage, string code);

    // 靜態方法
    public static OperationResult<T> Success(T data, string statusCode = "200");
    public static OperationResult<T> Failure(string errorMessage = "Operation Failed.", string statusCode = "400");
}
```

## 屬性說明

- `IsSuccess`: 布林值，表示操作是否成功。
- `Data`: 泛型資料，成功時包含結果資料，失敗時為預設值。
- `ErrorMessage`: 字串，失敗時包含錯誤訊息。
- `Code`: 字串，HTTP 狀態碼或自訂狀態碼。

## 使用方法

### 成功回應

```csharp
// 在控制器中
[HttpGet]
public OperationResult<WeatherForecast> GetWeather()
{
    var forecast = new WeatherForecast
    {
        Date = DateTime.Now,
        TemperatureC = 25,
        Summary = "Sunny"
    };

    return OperationResult<WeatherForecast>.Success(forecast);
}
```

### 失敗回應

```csharp
[HttpGet]
public OperationResult<WeatherForecast> GetWeather()
{
    // 模擬錯誤情況
    if (someCondition)
    {
        return OperationResult<WeatherForecast>.Failure("Unable to retrieve weather data.", "500");
    }

    // 正常處理...
}
```

### 自訂狀態碼

```csharp
// 成功時自訂狀態碼
return OperationResult<User>.Success(user, "201"); // 創建成功

// 失敗時自訂狀態碼
return OperationResult<User>.Failure("User not found.", "404");
```

## 在專案中的應用

在 `Dotnet10AISamples.Api` 專案中，所有控制器都應繼承 `ControllerBase` 並回傳 `OperationResult<T>` 以確保一致的 API 回應格式。

錯誤處理應整合到已註冊的 `ProblemDetails` 管道中。對於驗證失敗，請使用 `ValidationException`。

### 範例控制器

```csharp
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public OperationResult<IEnumerable<WeatherForecast>> Get()
    {
        var forecasts = new List<WeatherForecast>
        {
            new() { Date = DateTime.Now, TemperatureC = 20, Summary = "Cool" },
            new() { Date = DateTime.Now.AddDays(1), TemperatureC = 25, Summary = "Warm" }
        };

        return OperationResult<IEnumerable<WeatherForecast>>.Success(forecasts);
    }
}
```

## 最佳實踐

1. 總是在控制器方法中回傳 `OperationResult<T>`。
2. 使用適當的 HTTP 狀態碼。
3. 提供有意義的錯誤訊息。
4. 對於驗證錯誤，使用專門的驗證異常處理器。

## 相關檔案

- `Dotnet10AISamples.Api/Common/OperationResult.cs`: OperationResult 類別定義
- `Dotnet10AISamples.Api/Middlewares/`: 異常處理中介軟體
- `Dotnet10AISamples.Api/Controllers/`: 控制器範例
