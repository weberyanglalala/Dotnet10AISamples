# Admin User Initialization Guide

## 概述

本指南提供為 Dotnet10AISamples API 建立管理員使用者並初始化角色系統的完整步驟。完成後，您將能夠：

- 建立第一個管理員帳號
- 自動種子預設角色（User 和 Admin）
- 測試角色-based 授權系統
- 使用管理員權限管理其他使用者

## 前置需求

### 系統需求

- .NET 10.0 SDK
- SQL Server (LocalDB 或完整 SQL Server 實例)
- Visual Studio Code 或其他支援 .NET 的 IDE

### 專案狀態

- JWT 認證系統已實作完成
- 角色實體和關聯已建立
- 資料庫遷移已建立

### 環境設定

確保您的 `appsettings.json` 包含正確的資料庫連線字串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Dotnet10AISamples;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "your-256-bit-secret-key-here-make-it-long-and-secure",
    "Issuer": "Dotnet10AISamples.Api",
    "Audience": "Dotnet10AISamples.Api",
    "ExpiryInHours": 24
  }
}
```

## 步驟一：資料庫遷移與種子資料

### 1.1 套用資料庫遷移

開啟終端機並執行以下命令來套用資料庫遷移：

```bash
# 切換到專案根目錄
cd /Users/weberyangwork/Projects/Dotnet10AISamples

# 套用所有待處理的遷移
dotnet ef database update --project Dotnet10AISamples.Api
```

**預期輸出：**

```
Build started...
Build succeeded.
Applying migration '20251222033210_AddUserEntity'.
Applying migration '20251222044920_AddRolesAndUserRoles'.
Done.
```

### 1.2 啟動應用程式以種子預設角色

執行應用程式將自動種子預設角色：

```bash
# 啟動應用程式（開發模式）
dotnet run --project Dotnet10AISamples.Api
```

應用程式啟動時會自動：

- 套用任何未套用的遷移
- 種子預設角色（User 和 Admin）
- 顯示成功訊息

**預期輸出：**

```
info: Dotnet10AISamples.Api.Extensions.DatabaseExtensions[0]
      Migrations applied and default data seeded successfully
```

### 1.3 驗證角色已建立

使用 SQL Server Management Studio 或 Azure Data Studio 連接到資料庫，執行以下查詢：

```sql
SELECT Id, Name, Description, CreatedAt FROM Roles;
```

**預期結果：**

```
Id                 Name   Description                      CreatedAt
------------------ ------ -------------------------------- -------------------
role_user_default  User   Standard user role with basic access  2025-01-22 10:30:00
role_admin_default Admin  Administrator role with full access   2025-01-22 10:30:00
```

## 步驟二：建立第一個管理員使用者

### 方法一：使用 API 建立管理員（推薦）

#### 2.1.1 啟動應用程式

確保應用程式正在執行：

```bash
dotnet run --project Dotnet10AISamples.Api
```

#### 2.1.2 建立管理員使用者

使用 HTTP 客戶端（如 Postman、curl 或 VS Code REST Client）建立管理員使用者：

**請求：**

```
POST http://localhost:5000/api/users
Content-Type: application/json

{
  "email": "admin@dotnetsamples.com",
  "password": "AdminPass123!"
}
```

**使用 curl：**

```bash
curl -X POST "http://localhost:5000/api/users" \
     -H "Content-Type: application/json" \
     -d '{
       "email": "admin@dotnetsamples.com",
       "password": "AdminPass123!"
     }'
```

**成功回應（201 Created）：**

```json
{
  "data": {
    "id": "usr_12345678-1234-1234-1234-123456789abc",
    "email": "admin@dotnetsamples.com",
    "isActive": true,
    "createdAt": "2025-01-22T10:30:00Z",
    "updatedAt": "2025-01-22T10:30:00Z"
  },
  "message": "User created successfully",
  "code": 201
}
```

#### 2.1.3 指派 Admin 角色

使用管理員 ID 指派 Admin 角色：

**請求：**

```
POST http://localhost:5000/api/users/{userId}/roles
Content-Type: application/json

{
  "roleId": "role_admin_default"
}
```

**使用 curl（替換 {userId} 為實際的使用者 ID）：**

```bash
curl -X POST "http://localhost:5000/api/users/usr_12345678-1234-1234-1234-123456789abc/roles" \
     -H "Content-Type: application/json" \
     -d '{
       "roleId": "role_admin_default"
     }'
```

**成功回應（201 Created）：**

```json
{
  "message": "Role assigned successfully",
  "code": 201
}
```

### 方法二：直接資料庫插入（開發環境）

如果您無法使用 API，可以直接在資料庫中建立管理員使用者：

```sql
-- 1. 插入預設角色（如果尚未存在）
INSERT INTO Roles (Id, Name, Description, CreatedAt, UpdatedAt)
SELECT 'role_user_default', 'User', 'Standard user role with basic access', GETUTCDATE(), GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 'role_user_default');

INSERT INTO Roles (Id, Name, Description, CreatedAt, UpdatedAt)
SELECT 'role_admin_default', 'Admin', 'Administrator role with full access', GETUTCDATE(), GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 'role_admin_default');

-- 2. 插入管理員使用者
DECLARE @UserId NVARCHAR(40) = NEWID();
-- 注意：請使用 BCrypt 演算法產生 AdminPass123 的雜湊值替換下面的預留位置
-- 您可以使用線上工具或 C# 程式碼：BCrypt.Net.BCrypt.HashPassword("AdminPass123")
DECLARE @PasswordHash NVARCHAR(256) = '$2a$12$zjLx9evcp3UEHw0LUcZkdeylM.8f4M06sKelC9TuyFq7yNxb6NIB.'; -- 使用 BCrypt 雜湊 AdminPass123

INSERT INTO Users (Id, Username, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt)
VALUES (@UserId, 'admin', 'admin@dotnetsamples.com', @PasswordHash, 1, GETUTCDATE(), GETUTCDATE());

-- 3. 指派 Admin 角色
INSERT INTO UserRoles (UserId, RoleId, AssignedAt, AssignedBy)
VALUES (@UserId, 'role_admin_default', GETUTCDATE(), @UserId);

-- 4. 驗證
SELECT u.Username, u.Email, r.Name as RoleName
FROM Users u
JOIN UserRoles ur ON u.Id = ur.UserId
JOIN Roles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@dotnetsamples.com';
```

## 步驟三：測試管理員認證

### 3.1 管理員登入

測試管理員帳號是否能正常登入：

**請求：**

```
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@dotnetsamples.com",
  "password": "AdminPass123!"
}
```

**成功回應（200 OK）：**

```json
{
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "usr_12345678-1234-1234-1234-123456789abc",
      "email": "admin@dotnetsamples.com",
      "roles": [
        {
          "id": "role_admin_default",
          "name": "Admin",
          "description": "Administrator role with full access"
        }
      ]
    },
    "expiresAt": "2025-01-23T10:30:00Z"
  },
  "message": "Login successful",
  "code": 200
}
```

### 3.2 測試管理員權限

使用取得的 JWT token 測試管理員專屬端點：

**請求：**

```
GET http://localhost:5000/api/roles
Authorization: Bearer {your-jwt-token}
```

**成功回應（200 OK）：**

```json
{
  "data": {
    "items": [
      {
        "id": "role_user_default",
        "name": "User",
        "description": "Standard user role with basic access"
      },
      {
        "id": "role_admin_default",
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

### 3.3 測試使用者管理

測試管理員是否能查看所有使用者：

**請求：**

```
GET http://localhost:5000/api/users
Authorization: Bearer {your-jwt-token}
```

## 步驟四：建立測試使用者

### 4.1 建立一般使用者

使用管理員權限建立測試使用者：

**請求：**

```
POST http://localhost:5000/api/users
Content-Type: application/json

{
  "email": "testuser@example.com",
  "password": "TestPass123!"
}
```

### 4.2 指派 User 角色

為測試使用者指派 User 角色：

**請求：**

```
POST http://localhost:5000/api/users/{testUserId}/roles
Authorization: Bearer {admin-jwt-token}
Content-Type: application/json

{
  "roleId": "role_user_default"
}
```

### 4.3 測試一般使用者權限

使用測試使用者登入並測試權限：

**登入請求：**

```
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "testuser@example.com",
  "password": "TestPass123!"
}
```

**測試受限端點（應該失敗）：**

```
GET http://localhost:5000/api/roles
Authorization: Bearer {test-user-jwt-token}
```

**預期回應（403 Forbidden）：**

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.4",
  "title": "Forbidden",
  "status": 403,
  "detail": "Access denied"
}
```

## API 使用範例

### 管理員操作

#### 取得所有使用者

```bash
curl -X GET "http://localhost:5000/api/users" \
     -H "Authorization: Bearer {admin-token}"
```

#### 建立新使用者

```bash
curl -X POST "http://localhost:5000/api/users" \
     -H "Content-Type: application/json" \
     -d '{
       "email": "newuser@example.com",
       "password": "NewPass123!"
     }'
```

#### 指派角色給使用者

```bash
curl -X POST "http://localhost:5000/api/users/{userId}/roles" \
     -H "Authorization: Bearer {admin-token}" \
     -H "Content-Type: application/json" \
     -d '{
       "roleId": "role_user_default"
     }'
```

#### 移除使用者角色

```bash
curl -X DELETE "http://localhost:5000/api/users/{userId}/roles/{roleId}" \
     -H "Authorization: Bearer {admin-token}"
```

### 一般使用者操作

#### 取得個人資訊

```bash
curl -X GET "http://localhost:5000/api/auth/me" \
     -H "Authorization: Bearer {user-token}"
```

#### 更新個人資料

```bash
curl -X PUT "http://localhost:5000/api/users/{userId}" \
     -H "Authorization: Bearer {user-token}" \
     -H "Content-Type: application/json" \
     -d '{
       "email": "updated@example.com"
     }'
```

## 疑難排解

### 常見問題

#### 1. 遷移失敗

**問題：** `dotnet ef database update` 失敗
**解決方案：**

- 確認 SQL Server 正在執行
- 檢查連線字串是否正確
- 確保資料庫存在或有建立權限

#### 2. 角色種子失敗

**問題：** 應用程式啟動時沒有種子角色
**解決方案：**

- 檢查應用程式日誌中的錯誤訊息
- 確認資料庫連線正常
- 手動執行種子邏輯

#### 3. 認證失敗

**問題：** 登入失敗
**解決方案：**

- 確認使用者存在且密碼正確
- 檢查使用者是否啟用（IsActive = 1）
- 驗證 JWT 設定是否正確

#### 4. 授權失敗

**問題：** 403 Forbidden 錯誤
**解決方案：**

- 確認 JWT token 正確
- 檢查使用者是否有適當的角色
- 驗證角色名稱是否正確（區分大小寫）

#### 5. 資料庫連線問題

**問題：** 無法連接到資料庫
**解決方案：**

- 確認 SQL Server 服務正在執行
- 檢查連線字串
- 驗證防火牆設定

### 除錯命令

#### 檢查資料庫狀態

```sql
-- 檢查所有表格
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- 檢查角色資料
SELECT * FROM Roles;

-- 檢查使用者資料
SELECT * FROM Users;

-- 檢查使用者角色關聯
SELECT u.Email, r.Name as RoleName, ur.AssignedAt
FROM Users u
JOIN UserRoles ur ON u.Id = ur.UserId
JOIN Roles r ON ur.RoleId = r.Id;
```

#### 檢查應用程式日誌

應用程式會記錄詳細的錯誤資訊到控制台。啟動應用程式時注意：

- 遷移應用狀態
- 種子資料執行狀態
- 認證和授權錯誤

## 安全注意事項

### 密碼政策

- 最小長度：8 個字元
- 必須包含：大小寫字母、數字
- 建議包含：特殊字元

### JWT 安全性

- Token 有效期：24 小時
- 使用強密鑰（至少 256 位元）
- 在生產環境中使用 HTTPS
- 定期輪換密鑰

### 管理員帳號

- 使用強密碼
- 限制管理員帳號數量
- 定期檢查管理員活動
- 考慮實作雙因素認證

## 下一步

完成管理員初始化後，您可以：

1. **建立更多角色**：根據業務需求建立自訂角色
2. **實作角色權限**：在控制器中新增更細緻的權限控制
3. **新增使用者註冊流程**：實作公開註冊端點
4. **實作密碼重設功能**：新增忘記密碼和重設功能
5. **新增審計日誌**：記錄敏感操作的審計追蹤

---

**文件版本：** 1.0
**最後更新：** 2025-01-22
**適用專案：** Dotnet10AISamples API</content>
<parameter name="filePath">/Users/weberyangwork/Projects/Dotnet10AISamples/docs/init-admin.md
