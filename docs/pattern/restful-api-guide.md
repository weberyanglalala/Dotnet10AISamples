# RESTful API 入門指南

## 概述

RESTful API（Representational State Transfer API）是一種設計風格，用於建立網路服務。它利用 HTTP 協議的標準方法來進行資源的操作，提供了一種統一、簡單且可擴展的 API 設計方式。

## 什麼是 RESTful API？

RESTful API 是一種基於 REST（Representational State Transfer）架構風格的 Web API。REST 由 Roy Fielding 在 2000 年的博士論文中提出，它定義了一組約束條件，用於建立可擴展的網路服務。

### REST 的核心原則

1. **Client-Server Architecture**（客戶端-伺服器架構）

   - 客戶端和伺服器分離，各自獨立演進
   - 提高了可移植性和可擴展性

2. **Stateless**（無狀態）

   - 每個請求都是獨立的，包含所有必要資訊
   - 伺服器不儲存客戶端狀態
   - 提高了可靠性、可擴展性和可見性

3. **Cacheable**（可快取）

   - 回應必須隱式或顯式定義為可快取
   - 提高效能和效率

4. **Uniform Interface**（統一介面）

   - 資源通過統一的介面進行識別和操作
   - 簡化了架構並提高了互動的可見性

5. **Layered System**（分層系統）

   - 客戶端無法分辨是否直接連接到最終伺服器
   - 提高了可擴展性

6. **Code on Demand (Optional)**（按需代碼，可選）
   - 伺服器可以臨時擴展客戶端功能

## 為什麼要使用 RESTful API？

### 優勢

1. **簡單易用**

   - 使用標準 HTTP 方法
   - 無需學習複雜的協議

2. **可擴展性**

   - 無狀態設計便於水平擴展
   - 分層架構支援複雜系統

3. **可快取**

   - 利用 HTTP 快取機制提高效能
   - 減少伺服器負載

4. **統一介面**

   - 標準化的操作方式
   - 易於理解和維護

5. **語言無關**

   - 任何支援 HTTP 的語言都能使用
   - 提高了互通性

6. **資源導向**
   - 以資源為中心設計
   - 更符合業務邏輯

### 與其他 API 風格的比較

| 特性     | RESTful API | SOAP     | GraphQL |
| -------- | ----------- | -------- | ------- |
| 協議     | HTTP        | 多種     | HTTP    |
| 資料格式 | JSON/XML    | XML      | JSON    |
| 狀態     | 無狀態      | 可有狀態 | 無狀態  |
| 快取     | 支援        | 有限     | 有限    |
| 學習曲線 | 低          | 高       | 中      |
| 靈活性   | 中          | 低       | 高      |

## RESTful API 的基本概念

### 資源 (Resources)

資源是 RESTful API 的核心。每個資源都有一個唯一的 URI（Uniform Resource Identifier）。

**範例：**

- `/api/users` - 使用者資源集合
- `/api/users/123` - 特定使用者資源

### HTTP 方法 (HTTP Methods)

RESTful API 使用標準 HTTP 方法來操作資源：

| 方法   | 用途         | 範例                       |
| ------ | ------------ | -------------------------- |
| GET    | 取得資源     | 取得使用者列表或單一使用者 |
| POST   | 建立資源     | 建立新使用者               |
| PUT    | 更新資源     | 更新使用者資訊             |
| PATCH  | 部分更新資源 | 更新使用者部分欄位         |
| DELETE | 刪除資源     | 刪除使用者                 |

### HTTP 狀態碼 (HTTP Status Codes)

狀態碼表示請求的結果：

| 狀態碼 | 意義                  | 用途                 |
| ------ | --------------------- | -------------------- |
| 200    | OK                    | 請求成功             |
| 201    | Created               | 資源建立成功         |
| 204    | No Content            | 請求成功但無回應內容 |
| 400    | Bad Request           | 請求參數錯誤         |
| 401    | Unauthorized          | 未授權               |
| 403    | Forbidden             | 禁止訪問             |
| 404    | Not Found             | 資源不存在           |
| 409    | Conflict              | 資源衝突             |
| 500    | Internal Server Error | 伺服器錯誤           |

## RESTful API 設計原則

### 1. 使用名詞而非動詞

**正確：**

```
GET /api/users
POST /api/users
GET /api/users/123
PUT /api/users/123
DELETE /api/users/123
```

**錯誤：**

```
GET /api/getUsers
POST /api/createUser
GET /api/getUserById
```

### 2. 使用複數名詞

**正確：**

```
GET /api/users
POST /api/users
```

**錯誤：**

```
GET /api/user
POST /api/user
```

### 3. 使用巢狀資源表示關聯

**範例：**

```
GET /api/users/123/posts - 取得特定使用者的所有文章
GET /api/users/123/posts/456 - 取得特定使用者的特定文章
```

### 4. 使用查詢參數進行篩選、分頁和排序

**範例：**

```
GET /api/users?page=1&pageSize=10&isActive=true&search=john
```

### 5. 版本控制

**推薦方式：**

```
GET /api/v1/users
```

或使用 Header：

```
Accept: application/vnd.api.v1+json
```

## 實作範例

以使用者管理 API 為例，參考 `UsersController.cs` 和 `user.md` 文件：

### 資源定義

```csharp
// UserDto - 回應資料傳輸物件
public class UserDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// CreateUserDto - 建立使用者請求物件
public class CreateUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
}
```

### 控制器實作

```csharp
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

    // GET /api/users - 取得所有使用者
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserDto>>), 200)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null,
        [FromQuery] string search = "")
    {
        var result = await _userService.GetPaginatedUsersAsync(
            new UserQueryParameters { Page = page, PageSize = pageSize, IsActive = isActive, Search = search });

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

    // GET /api/users/{id} - 取得單一使用者
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser(string id)
    {
        var result = await _userService.GetUserByIdAsync(id);

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

    // POST /api/users - 建立新使用者
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserDto dto,
        IValidator<CreateUserDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);
        var result = await _userService.CreateUserAsync(dto);

        if (!result.IsSuccess)
        {
            return Problem(detail: result.ErrorMessage, statusCode: result.Code);
        }

        return CreatedAtAction(nameof(GetUser), new { id = result.Data.Id },
            new ApiResponse<UserDto>
            {
                Data = result.Data,
                Message = "User created successfully",
                Code = 201
            });
    }

    // PUT /api/users/{id} - 更新使用者
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateUser(
        string id,
        [FromBody] UpdateUserDto dto,
        IValidator<UpdateUserDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);
        var result = await _userService.UpdateUserAsync(id, dto);

        if (!result.IsSuccess)
        {
            return Problem(detail: result.ErrorMessage, statusCode: result.Code);
        }

        return Ok(new ApiResponse<UserDto>
        {
            Data = result.Data,
            Message = "User updated successfully",
            Code = 200
        });
    }

    // DELETE /api/users/{id} - 刪除使用者
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (!result.IsSuccess)
        {
            return Problem(detail: result.ErrorMessage, statusCode: result.Code);
        }

        return NoContent();
    }
}
```

### API 回應格式

所有成功回應都使用統一的 `ApiResponse<T>` 格式：

```json
{
  "data": {
    "id": "12345678-1234-1234-1234-123456789012",
    "username": "johndoe",
    "email": "john@example.com",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  },
  "message": "User retrieved successfully",
  "code": 200
}
```

分頁回應包含額外的分頁資訊：

```json
{
  "data": {
    "items": [...],
    "totalCount": 100,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 10
  },
  "message": "Users retrieved successfully",
  "code": 200
}
```

### 錯誤處理

錯誤使用標準的 ProblemDetails 格式：

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Username is required",
  "instance": "/api/users"
}
```

## 測試 RESTful API

### 使用 cURL 測試

```bash
# 取得所有使用者
curl -X GET "https://localhost:5001/api/users?page=1&pageSize=10" \
  -H "accept: application/json"

# 取得單一使用者
curl -X GET "https://localhost:5001/api/users/123" \
  -H "accept: application/json"

# 建立新使用者
curl -X POST "https://localhost:5001/api/users" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@example.com",
    "password": "SecurePass123!",
    "isActive": true
  }'

# 更新使用者
curl -X PUT "https://localhost:5001/api/users/123" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "isActive": false
  }'

# 刪除使用者
curl -X DELETE "https://localhost:5001/api/users/123" \
  -H "accept: application/json"
```

### 使用 Postman 或其他 API 測試工具

1. 設定請求方法 (GET, POST, PUT, DELETE)
2. 輸入 API URL
3. 設定 Headers (Content-Type: application/json)
4. 對於 POST/PUT 請求，設定請求主體
5. 發送請求並檢查回應

## 最佳實踐

### 1. 設計一致的資源命名

- 使用複數名詞：`/api/users` 而非 `/api/user`
- 使用小寫字母和連字號：`/api/user-profiles`
- 避免深層巢狀：`/api/users/123/posts/456/comments` 應考慮簡化

### 2. 適當使用 HTTP 狀態碼

- 200: 成功取得資源
- 201: 資源建立成功
- 204: 成功但無回應內容（刪除操作）
- 400: 請求參數錯誤
- 401: 未授權
- 403: 禁止訪問
- 404: 資源不存在
- 409: 資源衝突
- 500: 伺服器錯誤

### 3. 實作適當的驗證

- 使用資料註解或 FluentValidation
- 提供清晰的驗證錯誤訊息
- 在 API 文件中記錄驗證規則

### 4. 處理分頁和篩選

- 支援標準分頁參數：`page`, `pageSize`
- 提供搜尋和篩選功能
- 在回應中包含分頁資訊

### 5. 實作快取策略

- 使用適當的 Cache-Control headers
- 考慮使用 ETags 進行條件請求
- 快取靜態資源

### 6. 記錄和監控

- 記錄 API 請求和回應
- 監控效能指標
- 設定適當的錯誤追蹤

### 7. 文件化

- 使用 Swagger/OpenAPI 產生 API 文件
- 提供清晰的範例
- 記錄所有端點、參數和回應格式

## 常見陷阱和解決方案

### 1. 過度巢狀資源

**問題：** `/api/users/123/posts/456/comments/789`

**解決：** 考慮使用查詢參數或簡化資源結構

### 2. 不一致的錯誤處理

**問題：** 混合使用不同格式的錯誤回應

**解決：** 統一使用 ProblemDetails 格式

### 3. 忽略 HTTP 狀態碼

**問題：** 總是回傳 200，即使發生錯誤

**解決：** 使用適當的 HTTP 狀態碼表示操作結果

### 4. 缺乏版本控制

**問題：** API 變更破壞現有客戶端

**解決：** 實作 API 版本控制

### 5. 不安全的資源暴露

**問題：** 敏感資料意外暴露

**解決：** 實作適當的授權和資料過濾

## 總結

RESTful API 提供了一種標準化、簡單且強大的方式來設計網路服務。通過遵循 REST 原則，您可以建立：

- **可維護的**：統一介面和標準化操作
- **可擴展的**：無狀態設計和分層架構
- **高效的**：利用 HTTP 快取和標準方法
- **互通的**：語言無關和廣泛支援

開始設計 RESTful API 時，記住以下關鍵點：

1. 以資源為中心設計
2. 使用標準 HTTP 方法
3. 提供一致的回應格式
4. 實作適當的錯誤處理
5. 記錄您的 API

通過遵循這些原則和最佳實踐，您可以建立一個強大、可靠且易於使用的 API。

## 相關檔案

- `Dotnet10AISamples.Api/Controllers/Users/UsersController.cs`: 使用者控制器實作範例
- `docs/api/user.md`: 使用者 API 詳細文件
- `docs/pattern/api-response.md`: API 回應格式指南
- `docs/pattern/operation-result.md`: 服務層操作結果指南
- `docs/pattern/global-exception-handler.md`: 全域異常處理指南
