# JWT 認證實作計畫

## 概述

為 Dotnet10AISamples API 新增 JWT (JSON Web Token) 認證功能，保護使用者相關端點並提供安全的身份驗證機制。

## 背景

- 現有 UsersController 提供完整的 CRUD 操作，但缺乏身份驗證保護
- 需要保護敏感操作（如更新/刪除使用者）
- 提供標準的 JWT 認證流程：登入取得 token，後續請求攜帶 token
- 與現有的 OperationResult 錯誤處理模式整合

## 資料庫結構

### 資料表：`Users` (現有，不需新增 FirstName, LastName 欄位)

| 欄位         | 類型          | 約束條件              | 說明                   |
| ------------ | ------------- | --------------------- | ---------------------- |
| Id           | nvarchar(40)  | PRIMARY KEY, NOT NULL | 使用者 GUID            |
| Email        | nvarchar(256) | UNIQUE, NOT NULL      | 電子郵件 (用於登入)    |
| PasswordHash | nvarchar(256) | NOT NULL              | 密碼雜湊               |
| IsActive     | bit           | NOT NULL, DEFAULT 1   | 是否啟用               |
| CreatedAt    | datetime2     | NOT NULL              | 建立時間戳記 (UTC)     |
| UpdatedAt    | datetime2     | NOT NULL              | 最後更新時間戳記 (UTC) |

### 資料表：`Roles`

| 欄位        | 類型          | 約束條件              | 說明                   |
| ----------- | ------------- | --------------------- | ---------------------- |
| Id          | nvarchar(40)  | PRIMARY KEY, NOT NULL | 角色 GUID              |
| Name        | nvarchar(50)  | UNIQUE, NOT NULL      | 角色名稱 (User, Admin) |
| Description | nvarchar(200) | NULL                  | 角色描述               |
| CreatedAt   | datetime2     | NOT NULL              | 建立時間戳記 (UTC)     |
| UpdatedAt   | datetime2     | NOT NULL              | 最後更新時間戳記 (UTC) |

### 資料表：`UserRoles` (多對多關聯表)

| 欄位       | 類型         | 約束條件              | 說明               |
| ---------- | ------------ | --------------------- | ------------------ |
| UserId     | nvarchar(40) | FOREIGN KEY, NOT NULL | 使用者 ID          |
| RoleId     | nvarchar(40) | FOREIGN KEY, NOT NULL | 角色 ID            |
| AssignedAt | datetime2    | NOT NULL              | 指派時間戳記 (UTC) |
| AssignedBy | nvarchar(40) | FOREIGN KEY, NULL     | 指派者使用者 ID    |

### 索引

- 現有：IX_Users_IsActive
- 新增：IX_Users_Email (UNIQUE)
- 新增：IX_Roles_Name (UNIQUE)
- 新增：IX_UserRoles_UserId
- 新增：IX_UserRoles_RoleId
- 新增：IX_UserRoles_UserId_RoleId (UNIQUE)

### 關聯

- UserRoles.UserId → Users.Id (多對一)
- UserRoles.RoleId → Roles.Id (多對一)
- UserRoles.AssignedBy → Users.Id (多對一，可為空)

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
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "usr_12345678-1234-1234-1234-123456789abc",
    "email": "user@example.com",
    "roles": ["User"]
  },
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

**錯誤回應：**

- 401 Unauthorized: 無效憑證

### 2. GET /api/auth/me

**說明：** 取得目前登入使用者的資訊

**授權：** 需要有效 JWT token

**回應：** 200 OK

```json
{
  "id": "usr_12345678-1234-1234-1234-123456789abc",
  "email": "user@example.com",
  "roles": [
    {
      "id": "role_12345678-1234-1234-1234-123456789abc",
      "name": "User",
      "description": "Standard user role"
    }
  ]
}
```

### 3. GET /api/roles

**說明：** 取得所有可用角色

**授權：** Admin only

**回應：** 200 OK

```json
{
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
  ]
}
```

### 4. POST /api/users/{userId}/roles

**說明：** 指派角色給使用者

**授權：** Admin only

**請求主體：**

```json
{
  "roleId": "role_12345678-1234-1234-1234-123456789abc"
}
```

**回應：** 201 Created

### 5. DELETE /api/users/{userId}/roles/{roleId}

**說明：** 移除使用者的角色

**授權：** Admin only

**回應：** 204 No Content

### 修改現有 UsersController 端點

所有現有端點保持相同，但新增授權要求：

- `GET /api/users` - 需要 Admin 角色
- `GET /api/users/{id}` - 需要 Admin 角色或自己的資料
- `POST /api/users` - 公開（註冊）或需要 Admin 角色
- `PUT /api/users/{id}` - 需要 Admin 角色或自己的資料
- `DELETE /api/users/{id}` - 需要 Admin 角色

## 實作任務

### 第一階段：JWT 組態與中介軟體

- [x] 在 `appsettings.json` 新增 JWT 組態區段
- [x] 建立 `JwtSettings` 組態類別
- [x] 在 `Program.cs` 中註冊 JWT 認證服務
- [x] 設定 JWT Bearer token 驗證中介軟體
- [x] 新增角色-based 授權政策

### 第二階段：更新 User 實體與新增角色實體

- [x] 在 `User.cs` 新增 `Email`、`PasswordHash` 屬性（如果不存在）
- [x] 建立 `Role.cs` 實體，包含 Id, Name, Description, CreatedAt, UpdatedAt
- [x] 建立 `UserRole.cs` 實體，包含 UserId, RoleId, AssignedAt, AssignedBy
- [x] 新增 `UserRole` 列舉（User, Admin）用於程式碼常數
- [x] 更新 EF Core 映射，包含新欄位和索引
- [x] 在 `ApplicationDbContext` 中新增 `DbSet<Role>` 和 `DbSet<UserRole>`
- [x] 設定多對多關聯和外鍵約束
- [x] 建立資料庫遷移

### 第三階段：認證 DTOs 與驗證

- [x] 建立 `LoginRequestDto`（email, password）
- [x] 建立 `AuthResponseDto`（token, user, expiresAt）
- [x] 建立 `UserInfoDto`（用於 /me 端點，包含角色列表）
- [x] 建立 `RoleDto`（角色資訊）
- [x] 建立 `AssignRoleDto`（指派角色用）
- [x] 使用 FluentValidation 建立 `LoginRequestDtoValidator`
- [x] 更新現有 `CreateUserDto` 包含密碼欄位
- [x] 建立 `AssignRoleDtoValidator`

### 第四階段：認證與角色服務與資料存取

- [x] 建立 `IAuthService` 介面
- [x] 實作 `AuthService`，包含：
  - [x] `AuthenticateAsync(string email, string password)` - 驗證使用者並產生 JWT
  - [x] `GenerateJwtToken(User user)` - 產生 JWT token（包含角色宣告）
  - [x] `GetCurrentUserAsync()` - 從 JWT 取得目前使用者
  - [x] 使用 `IUserRepository` 進行使用者資料存取
  - [x] 密碼雜湊處理（使用 BCrypt 或 ASP.NET Core Identity）
- [x] 建立 `IRoleRepository` 介面 - 角色資料存取層
- [x] 實作 `RoleRepository`，包含資料存取方法
- [x] 建立 `IRoleService` 介面
- [x] 實作 `RoleService`，包含：
  - [x] `GetUserRolesAsync(string userId)` - 取得使用者角色
  - [x] `AssignRoleToUserAsync(string userId, string roleId, string assignedBy)` - 指派角色
  - [x] `RemoveRoleFromUserAsync(string userId, string roleId)` - 移除角色
  - [x] `GetAllRolesAsync()` - 取得所有可用角色
- [x] 在 `Program.cs` 中註冊服務和資料存取層

### 第五階段：認證與角色控制器

- [x] 建立 `AuthController`，包含：
  - [x] `POST /api/auth/login` - 登入端點
  - [x] `GET /api/auth/me` - 取得目前使用者資訊（包含角色）
- [x] 建立 `RolesController`，包含：
  - [x] `GET /api/roles` - 取得所有角色（Admin only）
  - [x] `POST /api/roles` - 建立新角色（Admin only）
  - [x] `POST /api/users/{userId}/roles` - 指派角色給使用者（Admin only）
  - [x] `DELETE /api/users/{userId}/roles/{roleId}` - 移除使用者角色（Admin only）
- [x] 整合 `OperationResult<T>` 錯誤處理

### 第六階段：更新 UsersController 授權

- [x] 在 `UsersController` 新增 `[Authorize]` 屬性到需要保護的端點
- [x] 新增角色-based 授權：`[Authorize(Roles = "Admin")]`
- [x] 實作自訂授權處理器以允許使用者存取自己的資料
- [x] 更新 `CreateUserAsync` 以處理密碼雜湊
- [x] 更新 `UpdateUserAsync` 以支援密碼更新

### 第七階段：更新服務層

- [x] 更新 `IUserService` 新增密碼處理方法
- [x] 更新 `UserService` 實作密碼雜湊和驗證
- [x] 確保密碼欄位不會在 DTOs 中序列化

### 第八階段：測試與驗證

- [ ] 建立認證整合測試
- [ ] 測試受保護端點的授權
- [ ] 測試 JWT token 驗證
- [ ] 測試角色-based 存取控制
- [x] 建立資料庫遷移（AddRolesAndUserRoles）
- [x] 新增預設角色種子資料（User, Admin）

### 第九階段：文件與建置

- [x] 更新 API 文件包含認證需求
- [x] 記錄 JWT 使用方式和範例
- [x] 執行完整建置和測試
- [x] 驗證所有元件正確註冊

## 驗證規則

### LoginRequestDto

- `Email`：必須是有效電子郵件格式，最大長度 256 字元
- `Password`：必須提供，最小長度 6 字元

### CreateUserDto (更新)

- `Email`：必須是有效電子郵件格式，最大長度 256 字元，全系統唯一
- `Password`：必須提供，最小長度 8 字元，包含大小寫字母和數字

### 業務規則

- 電子郵件必須在系統中唯一
- 只有 Admin 角色可以查看所有使用者
- 使用者可以查看和更新自己的資料
- 密碼在儲存時必須雜湊處理
- JWT token 有效期為 24 小時
- 停用的使用者無法登入
- 預設角色：新使用者自動指派 "User" 角色
- Admin 角色可以管理其他使用者的角色指派
- 角色指派記錄包含指派時間和指派者
- 使用者可以有多個角色（角色累加權限）

## 問題/決策

1. **Refresh Token 實作：**

   - 考慮的選項：實作 refresh token / 只使用 access token
   - 決策：先實作基本 JWT access token

2. **密碼雜湊演算法：**

   - 考慮的選項：BCrypt / ASP.NET Core Identity / PBKDF2
   - 決策：使用 BCrypt.Net-Next 提供安全且易用的密碼雜湊

3. **角色管理：**

   - 考慮的選項：簡單字串角色欄位 / 正規化角色表與多對多關聯
   - 決策：實作正規化角色系統，包含 Roles 和 UserRoles 表，以支援複雜權限管理和角色歷史追蹤

4. **註冊端點授權：**

   - 考慮的選項：公開註冊 / 僅限 Admin 建立使用者
   - 決策：公開註冊，但需要 Admin 核准（IsActive 欄位）

## 實作狀態

實作期間填寫：

**實作日期**：2025-01-22

### 關鍵規格

- **認證方式**：JWT Bearer Token
- **Token 有效期**：24 小時
- **雜湊演算法**：BCrypt
- **角色系統**：正規化多對多角色管理
- **預設角色**：User
- **管理角色**：Admin
- **端點**：/api/auth/_, /api/roles/_

### 建立/修改的檔案

列出實作期間建立或修改的所有檔案。

#### 新增檔案：

- `JwtSettings.cs` - JWT 組態類別
- `Role.cs` - 角色實體
- `UserRole.cs` - 使用者角色關聯實體
- `UserRoleType.cs` - 角色類型列舉
- `LoginRequestDto.cs` - 登入請求 DTO
- `AuthResponseDto.cs` - 認證回應 DTO
- `UserInfoDto.cs` - 使用者資訊 DTO（含角色）
- `RoleDto.cs` - 角色 DTO
- `AssignRoleDto.cs` - 指派角色 DTO
- `LoginRequestDtoValidator.cs` - 登入請求驗證器
- `AssignRoleDtoValidator.cs` - 指派角色驗證器
- `IAuthService.cs` - 認證服務介面
- `AuthService.cs` - 認證服務實作
- `IRoleRepository.cs` - 角色資料存取介面
- `RoleRepository.cs` - 角色資料存取實作
- `IRoleService.cs` - 角色服務介面
- `RoleService.cs` - 角色服務實作
- `AuthController.cs` - 認證控制器
- `RolesController.cs` - 角色管理控制器

#### 修改檔案：

- `appsettings.json` - 新增 JWT 組態區段
- `Program.cs` - 新增 JWT 認證和授權設定，註冊服務和資料存取層
- `User.cs` - 新增 Email 和 PasswordHash 屬性
- `CreateUserDto.cs` - 新增 Password 欄位
- `ApplicationDbContext.cs` - 新增 Role 和 UserRole DbSet，設定關聯
- `UsersController.cs` - 新增授權屬性和角色檢查
- `IUserService.cs` - 新增密碼處理方法
- `UserService.cs` - 實作密碼雜湊和驗證
- `DatabaseExtensions.cs` - 新增預設角色種子資料
- `jwt-authentication.md` - 更新實作狀態

### 建置狀態

記錄建置結果和已解決的任何問題。

**建置結果**：成功

- 專案編譯成功，無錯誤
- 3 個 nullable 參考型別警告（預期，因為專案設定為 disable nullable）

**已解決問題**：

- OperationResult.Failure 方法參數順序錯誤 - 已修正為 Failure(message, code)
- PaginatedResult.TotalPages 唯讀屬性 - 已移除手動指派
- 實體關聯設定 - 已正確設定多對多關聯和外鍵約束

### 後續步驟

- 測試建議
- 部署注意事項
- 未來增強功能
- **[Admin 初始化指南](init-admin.md)**：建立管理員使用者並設定角色系統的完整指南
