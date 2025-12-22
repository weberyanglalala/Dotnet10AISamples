# User API 文件

## 概述

User API 提供完整的 CRUD 操作，包含建立、讀取、更新、刪除使用者。所有端點都遵循 REST 設計原則，並使用統一的錯誤回應格式。

## API 端點

### 1. GET /users

**說明：** 列出所有使用者，支援分頁、篩選

**查詢參數：**

- `page`（選填，預設：1）：頁碼
- `pageSize`（選填，預設：10）：每頁項目數
- `isActive`（選填）：篩選啟用狀態 (true/false)
- `search`（選填）：搜尋使用者名稱或電子郵件

**授權：** Admin

**回應：** 200 OK

```json
{
  "items": [
    {
      "id": "12345678-1234-1234-1234-123456789012",
      "username": "johndoe",
      "email": "john@example.com",
      "isActive": true,
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-01T00:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10
}
```

### 2. GET /users/{id}

**說明：** 根據 ID 取得單一使用者

**參數：**

- `id`：使用者的 ID

**授權：** Admin 或自己的資料

**回應：** 200 OK

```json
{
  "id": "12345678-1234-1234-1234-123456789012",
  "username": "johndoe",
  "email": "john@example.com",
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### 3. POST /users

**說明：** 建立新的使用者

**授權：** Admin

**請求主體：**

```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "isActive": true
}
```

**回應：** 201 Created

```json
{
  "id": "12345678-1234-1234-1234-123456789012",
  "username": "johndoe",
  "email": "john@example.com",
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### 4. PUT /users/{id}

**說明：** 更新現有的使用者

**授權：** Admin 或自己的資料

**請求主體：**

```json
{
  "isActive": false
}
```

**回應：** 200 OK

### 5. DELETE /users/{id}

**說明：** 刪除使用者（軟刪除，設定 IsActive = false）

**授權：** Admin

**回應：** 204 No Content

## API 文件

### 端點總覽

| 方法   | 端點              | 說明                             | 授權  |
| ------ | ----------------- | -------------------------------- | ----- |
| GET    | `/api/users`      | 取得所有使用者（支援分頁和篩選） | Admin |
| GET    | `/api/users/{id}` | 根據 ID 取得單一使用者           | Admin |
| POST   | `/api/users`      | 建立新使用者                     | Admin |
| PUT    | `/api/users/{id}` | 更新現有使用者                   | Admin |
| DELETE | `/api/users/{id}` | 刪除使用者（軟刪除）             | Admin |

### 查詢參數

#### GET /api/users

| 參數       | 類型   | 必填 | 預設值 | 說明                     |
| ---------- | ------ | ---- | ------ | ------------------------ |
| `page`     | int    | 否   | 1      | 頁碼（從 1 開始）        |
| `pageSize` | int    | 否   | 10     | 每頁筆數（最大 100）     |
| `isActive` | bool   | 否   | null   | 篩選活躍狀態             |
| `search`   | string | 否   | null   | 搜尋使用者名稱或電子郵件 |

### 請求/回應格式

#### UserDto（回應）

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "username": "john_doe",
  "email": "john.doe@example.com",
  "isActive": true,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

#### CreateUserDto（請求）

```json
{
  "username": "john_doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!",
  "isActive": true
}
```

#### UpdateUserDto（請求）

```json
{
  "isActive": false
}
```

#### PaginatedResult<UserDto>（分頁回應）

```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "username": "john_doe",
      "email": "john.doe@example.com",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

### 端點詳細說明

#### 1. 取得所有使用者

**端點**：`GET /api/users`

**查詢參數**：

- `page` (int, optional): 頁碼，預設 1
- `pageSize` (int, optional): 每頁筆數，預設 10，最大 100
- `isActive` (bool, optional): 篩選活躍狀態
- `search` (string, optional): 搜尋使用者名稱或電子郵件

**回應**：

- `200 OK`: 成功，回傳分頁結果
- `500 Internal Server Error`: 伺服器錯誤

**cURL 範例**：

```bash
curl -X GET "https://localhost:5001/api/users?page=1&pageSize=10&isActive=true&search=john" \
  -H "accept: application/json"
```

#### 2. 取得單一使用者

**端點**：`GET /api/users/{id}`

**路徑參數**：

- `id` (string, required): 使用者 ID（GUID 格式）

**回應**：

- `200 OK`: 成功，回傳使用者資料
- `404 Not Found`: 使用者不存在
- `500 Internal Server Error`: 伺服器錯誤

**cURL 範例**：

```bash
curl -X GET "https://localhost:5001/api/users/550e8400-e29b-41d4-a716-446655440000" \
  -H "accept: application/json"
```

#### 3. 建立新使用者

**端點**：`POST /api/users`

**請求主體**：CreateUserDto

**回應**：

- `201 Created`: 成功建立，回傳新使用者資料
- `400 Bad Request`: 請求資料無效
- `409 Conflict`: 使用者名稱或電子郵件已存在
- `500 Internal Server Error`: 伺服器錯誤

**cURL 範例**：

```bash
curl -X POST "https://localhost:5001/api/users" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john.doe@example.com",
    "password": "SecurePass123!",
    "isActive": true
  }'
```

#### 4. 更新使用者

**端點**：`PUT /api/users/{id}`

**路徑參數**：

- `id` (string, required): 使用者 ID（GUID 格式）

**請求主體**：UpdateUserDto

**回應**：

- `200 OK`: 成功更新，回傳更新後的使用者資料
- `400 Bad Request`: 請求資料無效
- `404 Not Found`: 使用者不存在
- `500 Internal Server Error`: 伺服器錯誤

**cURL 範例**：

```bash
curl -X PUT "https://localhost:5001/api/users/550e8400-e29b-41d4-a716-446655440000" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "isActive": false
  }'
```

#### 5. 刪除使用者

**端點**：`DELETE /api/users/{id}`

**路徑參數**：

- `id` (string, required): 使用者 ID（GUID 格式）

**回應**：

- `204 No Content`: 成功刪除
- `404 Not Found`: 使用者不存在
- `500 Internal Server Error`: 伺服器錯誤

**cURL 範例**：

```bash
curl -X DELETE "https://localhost:5001/api/users/550e8400-e29b-41d4-a716-446655440000" \
  -H "accept: application/json"
```

### 錯誤回應格式

所有錯誤回應都使用標準的 ProblemDetails 格式：

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "錯誤訊息描述",
  "instance": "/api/users"
}
```

### 常見錯誤情況

| HTTP 狀態碼 | 錯誤訊息              | 說明                     |
| ----------- | --------------------- | ------------------------ |
| 400         | Bad Request           | 請求資料驗證失敗         |
| 404         | Not Found             | 資源不存在               |
| 409         | Conflict              | 使用者名稱或電子郵件重複 |
| 500         | Internal Server Error | 伺服器內部錯誤           |

## 驗證規則

### CreateUserDto

- `Username`：必填，長度 3-50 字元，只能包含字母、數字、底線
- `Email`：必填，有效的電子郵件格式
- `Password`：必填，至少 8 字元，包含大小寫字母、數字、特殊字元
- `IsActive`：選填，預設 true

### UpdateUserDto

- `IsActive`：選填

### 業務規則

- 使用者名稱必須唯一（不區分大小寫）
- 電子郵件必須唯一（不區分大小寫）
- 密碼會自動雜湊處理，不會在回應中返回
- 刪除操作為軟刪除（設定 IsActive = false）
- 所有時間戳記使用 UTC 格式
