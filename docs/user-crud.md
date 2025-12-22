# User CRUD 實作計畫

## 概述

實作完整的 User 實體 CRUD 操作，包含建立、讀取、更新、刪除使用者。使用 SQL Server 作為資料庫提供者，建立 ApplicationDbContext 並設定 Entity Framework Core。

## 背景

- 此專案目前沒有使用者管理功能，需要新增基本的 User 實體
- 使用 SQL Server 作為資料庫提供者，與 .NET 10 相容
- 遵循專案的架構模式：Controller -> Service -> Repository -> DbContext
- 使用 OperationResult<T> 作為服務層統一回應格式，ApiResponse<T> 作為 API 回應格式

## 資料庫結構

### 資料表：`Users`

| 欄位         | 類型          | 約束條件              | 說明                   |
| ------------ | ------------- | --------------------- | ---------------------- |
| Id           | nvarchar(40)  | PRIMARY KEY, NOT NULL | GUID                   |
| Username     | nvarchar(50)  | UNIQUE, NOT NULL      | 使用者名稱，唯一       |
| Email        | nvarchar(255) | UNIQUE, NOT NULL      | 電子郵件，唯一         |
| PasswordHash | nvarchar(255) | NOT NULL              | 密碼雜湊               |
| IsActive     | bit           | NOT NULL, DEFAULT 1   | 是否啟用               |
| CreatedAt    | datetime2     | NOT NULL              | 建立時間戳記 (UTC)     |
| UpdatedAt    | datetime2     | NOT NULL              | 最後更新時間戳記 (UTC) |

### 索引

- UNIQUE INDEX on Username
- UNIQUE INDEX on Email
- INDEX on IsActive (for filtering active users)

### 關聯

- 目前無外鍵關聯

## 實作任務

### 第一階段：Entity Framework Core 設定

- [x] 安裝必要的 NuGet 套件：
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Tools
  - Microsoft.EntityFrameworkCore.Design
- [x] 建立 `ApplicationDbContext` 類別
- [x] 在 `Program.cs` 中設定 SQL Server 連線字串
- [x] 註冊 DbContext 服務

### 第二階段：實體與資料庫

- [x] 建立 `User` 實體，包含屬性：Id, Username, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
- [x] 在 `ApplicationDbContext` 中新增 `DbSet<User>`
- [x] 在 `OnModelCreating` 中設定實體的索引、資料表名稱和約束
- [x] 為新資料表建立遷移
- [x] 執行遷移以建立/更新資料庫

### 第三階段：DTOs 與驗證

- [x] 建立 `UserDto`（回應 DTO）
- [x] 建立 `CreateUserDto`（用於 POST 請求）
- [x] 建立 `UpdateUserDto`（用於 PUT 請求）
- [x] 建立 `UserQueryParameters`（用於 GET 列表篩選）
- [x] 建立 `UserMappings`（用於實體/DTO 對應）
- [x] 使用 FluentValidation 規則建立 `CreateUserDtoValidator`
- [x] 使用 FluentValidation 規則建立 `UpdateUserDtoValidator`

### 第四階段：儲存庫層

- [x] 建立 `IUserRepository` 介面
- [x] 實作 `UserRepository`，包含以下方法：
  - [x] `GetAllUsersAsync(UserQueryParameters)` - 支援分頁、篩選
  - [x] `GetUserByIdAsync(string id)`
  - [x] `GetUserByUsernameAsync(string username)`
  - [x] `GetUserByEmailAsync(string email)`
  - [x] `CreateUserAsync(User)`
  - [x] `UpdateUserAsync(User)`
  - [x] `DeleteUserAsync(string id)`（軟刪除）
  - [x] `UserExistsAsync(string id)`

### 第五階段：服務層

- [x] 建立 `IUserService` 介面
- [x] 實作 `UserService`，包含業務邏輯：
  - [x] 密碼雜湊處理（使用 BCrypt 或類似）
  - [x] 驗證使用者名稱和電子郵件唯一性
  - [x] 處理 `OperationResult<T>` 回應
  - [x] 更新時更新 `UpdatedAt` 時間戳記
  - [x] 錯誤處理和日誌記錄

### 第六階段：控制器

- [x] 建立 `UsersController`，包含以下端點：
  - [x] `GET /users` - 列出所有，支援查詢
  - [x] `GET /users/{id}` - 根據 ID 取得單一
  - [x] `POST /users` - 建立新的
  - [x] `PUT /users/{id}` - 更新
  - [x] `DELETE /users/{id}` - 軟刪除
- [x] 新增授權屬性（Admin 權限）
- [x] 新增 FluentValidation 整合

### 第七階段：服務註冊

- [x] 在 `Program.cs` 中註冊 `IUserRepository` 和實作
- [x] 在 `Program.cs` 中註冊 `IUserService` 和實作
- [x] 為依賴注入註冊驗證器

### 第八階段：文件

- [x] 在 `docs/user-crud.md` 建立繁體中文 API 文件
- [x] 包含端點說明、範例、錯誤情況
- [x] 新增請求/回應範例
- [x] 包含 cURL 範例

## API 文件

詳細的 API 文件請參考：[docs/api/user.md](docs/api/user.md)

該文件包含完整的端點說明、請求/回應格式、驗證規則和 cURL 範例.

## 問題/決策

1. **密碼雜湊演算法：**

   - 考慮的選項：BCrypt, Argon2, PBKDF2
   - 決策：使用 BCrypt.NET，因為它簡單、安全且廣泛使用

2. **授權策略：**

   - 考慮的選項：簡單角色-based, JWT claims, 自訂政策
   - 決策：使用簡單的角色-based 授權（Admin），之後可擴展

3. **軟刪除 vs 硬刪除：**

   - 考慮的選項：硬刪除（永久移除）, 軟刪除（標記為非活躍）
   - 決策：使用軟刪除以保留資料完整性和稽核追蹤

4. **ID 格式：**
   - 考慮的選項：純 GUID, 前綴 GUID, 數字 ID
   - 決策：使用純 GUID 以保持簡潔

## 實作狀態

實作期間填寫：

**實作日期**：2024-12-19

### 關鍵規格

- **實體名稱**：`User`
- **資料表名稱**：`Users`
- **ID 前綴**：無（直接使用 GUID）
- **端點**：`/users`
- **授權**：Admin

### 建立/修改的檔案

#### Phase 1: EF Core 設定 ✅

- `Dotnet10AISamples.Api.csproj` - 新增 EF Core 和 SQL Server 套件
- `appsettings.json` - 新增 SQL Server 連線字串

#### Phase 2: 實體/資料庫 ✅

- `Entities/User.cs` - 使用者實體類別（含 XML 文件）
- `Data/ApplicationDbContext.cs` - DbContext 設定和索引
- `Data/Migrations/` - EF Core 遷移檔案

#### Phase 3: DTOs/驗證 ✅

- `DTOs/UserDto.cs` - 使用者回應 DTO
- `DTOs/CreateUserDto.cs` - 建立使用者請求 DTO
- `DTOs/UpdateUserDto.cs` - 更新使用者請求 DTO
- `DTOs/UserQueryParameters.cs` - 查詢參數 DTO
- `Validators/CreateUserValidator.cs` - 建立使用者驗證器
- `Validators/UpdateUserValidator.cs` - 更新使用者驗證器

#### Phase 4: Repository 層 ✅

- `Repositories/IUserRepository.cs` - 使用者倉儲介面
- `Repositories/UserRepository.cs` - 使用者倉儲實作

#### Phase 5: Service 層 ✅

- `Services/IUserService.cs` - 使用者服務介面
- `Services/UserService.cs` - 使用者服務實作（含密碼雜湊、驗證）

#### Phase 6: Controllers ✅

- `Controllers/UsersController.cs` - REST API 控制器（含所有 CRUD 端點）

#### Phase 7: 相依性注入 ✅

- `Program.cs` - 註冊所有服務和倉儲

#### 共用元件

- `Common/PaginatedResult.cs` - 分頁結果類別
- `Mappings/UserMappings.cs` - 實體-DTO 對應

### 建置狀態

✅ **建置成功** - 無編譯錯誤或警告

- 解決了 nullable 參考型別註釋警告（CS8632）
- 更新 AGENTS.md 編碼指南以避免在 nullable 停用時使用 nullable 註釋

### 後續步驟

#### Phase 8: 文件 ✅

- 更新設計文件實作狀態

#### Phase 9: 建置/驗證

- 執行資料庫遷移
- 測試所有 API 端點
- 驗證密碼雜湊和驗證邏輯

#### 測試建議

1. 使用 Swagger/OpenAPI 測試所有端點
2. 驗證分頁和篩選功能
3. 測試重複使用者名稱/電子郵件處理
4. 確認軟刪除行為

#### 部署注意事項

- 確保 SQL Server 可用且連線字串正確
- 執行 `dotnet ef database update` 套用遷移
- 設定適當的環境變數

#### 未來增強功能

- JWT 身份驗證
- 角色-based 授權
- 使用者個人資料圖片上傳
- 密碼重設功能
- 登入記錄追蹤
