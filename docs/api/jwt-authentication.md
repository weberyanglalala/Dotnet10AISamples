# JWT Authentication API 文件

## 概述

JWT Authentication API 提供安全的身份驗證和授權功能，使用 JSON Web Tokens (JWT) 進行使用者認證。支援角色-based 存取控制，包含標準使用者 (User) 和管理員 (Admin) 角色。

## 認證流程

1. 使用者使用電子郵件和密碼登入 (`POST /api/auth/login`)
2. 伺服器驗證憑證並返回 JWT token
3. 後續請求在 Authorization header 中攜帶 `Bearer {token}`
4. Token 有效期為 24 小時

## API 端點

### 1. POST /api/auth/login

**說明：** 使用者登入，驗證憑證並返回 JWT token

**授權：** 公開

**請求主體：**

```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**回應：** 200 OK

```json
{
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "usr_12345678-1234-1234-1234-123456789abc",
      "email": "user@example.com",
      "roles": ["User"]
    },
    "expiresAt": "2024-01-01T12:00:00Z"
  },
  "message": "Login successful",
  "code": 200
}
```

**錯誤回應：**

- 400 Bad Request: 請求格式錯誤
- 401 Unauthorized: 無效憑證或帳號未啟用

### 2. GET /api/auth/me

**說明：** 取得目前登入使用者的資訊

**授權：** 需要有效 JWT token

**回應：** 200 OK

```json
{
  "data": {
    "id": "usr_12345678-1234-1234-1234-123456789abc",
    "email": "user@example.com",
    "roles": [
      {
        "id": "role_12345678-1234-1234-1234-123456789abc",
        "name": "User",
        "description": "Standard user role"
      }
    ]
  },
  "message": "User info retrieved successfully",
  "code": 200
}
```

**錯誤回應：**

- 401 Unauthorized: Token 無效或過期

### 3. GET /api/roles

**說明：** 取得所有可用角色

**授權：** Admin only

**回應：** 200 OK

```json
{
  "data": {
    "items": [
      {
        "id": "role_12345678-1234-1234-1234-123456789abc",
        "name": "User",
        "description": "Standard user role"
      },
      {
        "id": "role_87654321-4321-4321-4321-cba987654321",
        "name": "Admin",
        "description": "Administrator role with full access"
      }
    ],
    "totalCount": 2,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "message": "Roles retrieved successfully",
  "code": 200
}
```

**錯誤回應：**

- 403 Forbidden: 權限不足

### 4. POST /api/users/{userId}/roles

**說明：** 指派角色給使用者

**參數：**

- `userId`：目標使用者 ID

**授權：** Admin only

**請求主體：**

```json
{
  "roleId": "role_12345678-1234-1234-1234-123456789abc"
}
```

**回應：** 201 Created

**錯誤回應：**

- 400 Bad Request: 請求格式錯誤或角色不存在
- 403 Forbidden: 權限不足
- 404 Not Found: 使用者不存在

### 5. DELETE /api/users/{userId}/roles/{roleId}

**說明：** 移除使用者的角色

**參數：**

- `userId`：目標使用者 ID
- `roleId`：要移除的角色 ID

**授權：** Admin only

**回應：** 204 No Content

**錯誤回應：**

- 403 Forbidden: 權限不足
- 404 Not Found: 使用者或角色不存在

## 授權標頭

所有受保護的端點都需要在請求標頭中包含：

```
Authorization: Bearer {jwt_token}
```

## 角色權限

- **User**: 基本使用者權限，可以存取自己的資料
- **Admin**: 管理員權限，可以管理所有使用者和角色

## 錯誤處理

所有端點都使用統一的錯誤回應格式：

```json
{
  "isSuccess": false,
  "errorMessage": "錯誤訊息",
  "code": 400
}
```

## 使用範例

### 登入取得 Token

```bash
curl -X POST https://api.example.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "password123"}'
```

### 使用 Token 存取受保護端點

```bash
curl -X GET https://api.example.com/api/auth/me \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### 管理員指派角色

```bash
curl -X POST https://api.example.com/api/users/usr_123/roles \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{"roleId": "role_admin"}'
```
