# 設計新功能: $arguments

一個用於在專案實作前，為新功能建立詳細設計文件的完整範本。

## 目的

此指令確保所有新功能都經過完整規劃，包含：
- 詳細的設計文件
- 分階段實作任務與未勾選的待辦事項
- 清晰的資料庫結構與 API 規格
- 實作前取得使用者同意

## 流程

### 步驟 1：建立設計文件

在 `docs/` 目錄下建立新的 markdown 檔案，使用以下結構：

```markdown
# {功能名稱} 實作計畫

## 概述
功能及其目的的簡要說明。

## 背景
- 為什麼需要此功能的背景說明
- 現有相關功能或實體
- 任何命名衝突或注意事項

## 資料庫結構

### 資料表：`{table_name}`
| 欄位        | 類型         | 約束條件                       | 說明                           |
|-------------|--------------|--------------------------------|--------------------------------|
| Id         | nvarchar(40)  | PRIMARY KEY, NOT NULL          | 前綴 GUID：`{Guid}`   |
| ...         | ...          | ...                            | ...                            |
| CreatedAt  | datetime2    | NOT NULL         | 建立時間戳記 (UTC)             |
| UpdatedAt  | datetime2    | NOT NULL        | 最後更新時間戳記 (UTC)         |

### 索引
- 列出所有索引（唯一、非唯一、複合）

### 關聯
- 與其他資料表的外鍵關係
- 導航屬性

## API 端點

### 1. GET /{resource-path}
**說明：** 列出所有 {實體}，支援分頁、篩選

**查詢參數：**
- `page`（選填，預設：1）：頁碼
- `pageSize`（選填，預設：10）：每頁項目數
- {新增自訂查詢參數}

**授權：** {Admin/User/Public}

**回應：** 200 OK
```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10
}
```

### 2. GET /{resource-path}/{id}
**說明：** 根據 ID 取得單一 {實體}

**參數：**
- `id`：{實體} 的 ID

**查詢參數：**
- `fields`（選填）：欄位選擇

**授權：** {Admin/User/Public}

**回應：** 200 OK
```json
{
  "id": "...",
  ...
}
```

### 3. POST /{resource-path}
**說明：** 建立新的 {實體}

**授權：** {Admin/User}

**請求主體：**
```json
{
  "field1": "value1",
  "field2": "value2"
}
```

**回應：** 201 Created

### 4. PUT/PATCH /{resource-path}/{id}
**說明：** 更新現有的 {實體}

**授權：** {Admin/User}

**請求主體：**
```json
{
  "field1": "updated value"
}
```

**回應：** 200 OK / 204 No Content

### 5. DELETE /{resource-path}/{id}（如適用）
**說明：** 刪除 {實體}

**授權：** {僅限 Admin}

**回應：** 204 No Content

## 實作任務

### 第一階段：實體與資料庫
- [ ] 建立 `{EntityName}` 實體，包含屬性：{列出屬性}
- [ ] 在 `ApplicationDbContext` 中新增 `DbSet<{EntityName}>`
- [ ] 在 `OnModelCreating` 中設定實體的索引、資料表名稱和關聯
- [ ] 為新資料表建立遷移
- [ ] 執行遷移以建立/更新資料庫

### 第二階段：DTOs 與驗證
- [ ] 建立 `{EntityName}Dto`（回應 DTO，支援 HATEOAS）
- [ ] 建立 `Create{EntityName}Dto`（用於 POST 請求）
- [ ] 建立 `Update{EntityName}Dto`（用於 PUT/PATCH 請求）
- [ ] 建立 `{EntityName}QueryParameters`（用於 GET 列表篩選）
- [ ] 建立 `{EntityName}Queries`（用於 EF Core 投影）
- [ ] 建立 `{EntityName}Mappings`（用於實體/DTO 對應和排序對應）
- [ ] 使用 FluentValidation 規則建立 `Create{EntityName}DtoValidator`
- [ ] 使用 FluentValidation 規則建立 `Update{EntityName}DtoValidator`

### 第三階段：儲存庫層
- [ ] 建立 `I{EntityName}Repository` 介面
- [ ] 實作 `{EntityName}Repository`，包含以下方法：
    - [ ] `GetAll{EntityName}Async()` - 支援分頁、篩選
    - [ ] `Get{EntityName}ByIdAsync(string id)`
    - [ ] `Create{EntityName}Async({EntityName})`
    - [ ] `Update{EntityName}Async({EntityName})`
    - [ ] `Delete{EntityName}Async(string id)`（如適用）
    - [ ] `{EntityName}ExistsAsync(string id)`
    - [ ] 依需要新增自訂查詢方法
    - [ ] **對於需要原子性的複雜操作**（例如：刪除 + 新增）：
        - [ ] 建立單一儲存庫方法來封裝整個操作
        - [ ] 使用 `await using var transaction = await dbContext.Database.BeginTransactionAsync()` 處理交易
        - [ ] 包含 `try-catch`，成功時呼叫 `CommitAsync()`，失敗時呼叫 `RollbackAsync()`
        - [ ] 方法應拋出例外；服務層會捕捉並轉換為 `OperationResult`

### 第四階段：服務層
- [ ] 建立 `I{EntityName}Service` 介面
- [ ] 實作 `{EntityName}Service`，包含業務邏輯：
    - [ ] 驗證業務規則（唯一性、約束）
    - [ ] 處理 `OperationResult<T>` 回應
    - [ ] 更新時更新 `UpdatedAt` 時間戳記
    - [ ] 錯誤處理和日誌記錄
    - [ ] 複雜業務邏輯（如有需要）
    - [ ] **重要**：服務層絕不管理資料庫交易
    - [ ] **絕不**在服務層呼叫 `BeginTransactionAsync()`、`CommitAsync()` 或 `RollbackAsync()`
    - [ ] 協調儲存庫呼叫並將例外轉換為 `OperationResult<T>`

### 第五階段：控制器
- [ ] 建立 `{EntityName}Controller`，包含以下端點：
    - [ ] `GET /{resource-path}` - 列出所有，支援查詢
    - [ ] `GET /{resource-path}/{id}` - 根據 ID 取得單一
    - [ ] `POST /{resource-path}` - 建立新的
    - [ ] `PUT/PATCH /{resource-path}/{id}` - 更新
    - [ ] `DELETE /{resource-path}/{id}` - 刪除（如適用）
- [ ] 新增 HATEOAS 連結產生
- [ ] 新增欄位選擇支援
- [ ] 新增授權屬性
- [ ] 新增 FluentValidation 整合

### 第六階段：服務註冊
- [ ] 在 `DependencyInjection.cs` 中註冊 `I{EntityName}Repository` 和實作
- [ ] 在 `DependencyInjection.cs` 中註冊 `I{EntityName}Service` 和實作
- [ ] 為 {EntityName} 註冊排序對應提供者
- [ ] 為依賴注入註冊驗證器

### 第七階段：文件
- [ ] 在 `docs/{feature-path}/{feature-name}.md` 建立繁體中文 API 文件
- [ ] 包含端點說明、範例、錯誤情況
- [ ] 新增請求/回應範例
- [ ] 包含 cURL 範例
- [ ] 為複雜操作新增 Mermaid 流程圖
- [ ] 新增 HATEOAS 格式範例
- [ ] 包含驗證規則和業務規則文件

### 第八階段：測試（如有需要）
- [ ] 為所有端點建立整合測試
- [ ] 測試不同角色的授權
- [ ] 測試驗證規則
- [ ] 測試業務邏輯邊界情況
- [ ] 測試 HATEOAS 連結產生
- [ ] 測試欄位選擇
- [ ] 測試分頁和排序

### 第九階段：建置與驗證
- [ ] 執行資料庫遷移
- [ ] 成功建置專案（零錯誤）
- [ ] 驗證所有元件已正確註冊
- [ ] 手動測試端點

## 驗證規則

### Create{EntityName}Dto
- `Field1`：{約束}
- `Field2`：{約束}

### Update{EntityName}Dto
- `Field1`：{提供時的約束}
- `Field2`：{提供時的約束}

### 業務規則
- 列出所有業務規則（唯一性、依賴性、狀態轉換）
- 記錄任何級聯行為
- 記錄角色存取控制以外的授權規則

## 問題/決策

1. **問題 1：**
    - 考慮的選項
    - 決策：待定 / {決策}

2. **問題 2：**
    - 考慮的選項
    - 決策：待定 / {決策}

## 實作狀態

實作期間填寫：

**實作日期**：{日期}

### 關鍵規格
- **實體名稱**：`{EntityName}`
- **資料表名稱**：`{table_name}`
- **ID 前綴**：`{prefix}_`
- **端點**：`/{resource-path}`
- **授權**：{角色}

### 建立/修改的檔案
列出實作期間建立或修改的所有檔案。

### 建置狀態
記錄建置結果和已解決的任何問題。

### 後續步驟
- 測試建議
- 部署注意事項
- 未來增強功能
```

### 步驟 2：與使用者審查設計

在進行實作之前：

1. **向使用者呈現設計文件**
2. **強調問題/決策區段中的任何未決問題**
3. **取得使用者對以下項目的同意**：
   - 資料庫結構（資料表名稱、欄位、索引）
   - API 端點（路由、方法、授權）
   - 實作階段
   - 任何需要的決策

4. **針對任何模糊的需求提出澄清問題**

### 步驟 3：根據回饋更新設計

- 根據使用者的決策更新設計文件
- 將問題標記為「已解決」並附上選擇的方案
- 確保所有待辦事項保持**未勾選**（使用 `- [ ]`）

### 步驟 4：取得最終同意

詢問使用者：「設計已完成。是否可以開始實作？」

只有在收到明確同意（例如：「go」、「proceed」、「yes」、「開始」）後才進行實作

### 步驟 5：實作

一旦獲得同意，按照實作階段進行：

1. 依序完成每個階段
2. 完成任務後使用 `- [x]` 標記為已完成
3. 如果發現任何偏差或問題，更新設計文件
4. 在主要階段後建置專案
5. 記錄任何錯誤及其解決方案

### 步驟 6：最終文件更新

實作完成後：

1. 使用以下內容更新「實作狀態」區段：
   - 實作日期
   - 所有建立/修改的檔案清單
   - 建置狀態（已解決的錯誤/警告）
   - 測試和部署的後續步驟

2. 將所有階段標記為 ✅ 已完成

3. 建立/更新繁體中文 API 文件

## 設計文件檢查清單

在請求使用者同意之前，確保設計包含：

- [x] 清晰的概述和背景
- [x] 完整的資料庫結構，包含所有欄位、類型、約束
- [x] 所有索引已記錄（唯一、非唯一、複合）
- [x] 外鍵關係已定義
- [x] API 端點包含完整的請求/回應範例
- [x] 每個端點的授權需求
- [x] 實作任務按階段分解（8-9 個階段）
- [x] 所有任務標記為**未勾選** `- [ ]`
- [x] 驗證規則已記錄
- [x] 業務規則已記錄
- [x] 問題/決策區段包含任何未決問題
- [x] 實作狀態區段的預留位置

## 常見實體模式

### 標準欄位
所有實體通常包含：
- `id`：VARCHAR(40)，PRIMARY KEY，前綴 GUID
- `created_at`：TIMESTAMP，NOT NULL，DEFAULT now()
- `updated_at`：TIMESTAMP，NOT NULL，DEFAULT now()

### 命名慣例
- **資料庫**：資料表和欄位使用 snake_case
- **C# 實體**：類別和屬性名稱使用 PascalCase
- **API 路由**：使用 kebab-case（例如：`/image-model-types`）
- **DTOs**：使用 PascalCase 加後綴（Dto、QueryParameters）

### 標準查詢功能
所有列表端點通常支援：
- 分頁（`page`、`pageSize`）
- 自訂篩選（實體特定的查詢參數）

### 授權模式
- 僅限 Admin：`[Authorize(Roles = $"{UserRoles.Admin}")]`
- User 或 Admin：`[Authorize(Roles = $"{UserRoles.User},{UserRoles.Admin}")]`
- 已驗證：`[Authorize]`
- 公開：無屬性

### 回應模式
- 列表端點：`PaginationResult<T>` 包含項目、分頁中繼資料、連結
- 單一項目：實體 DTO 包含選用連結
- 建立：201 Created 包含 Location 標頭
- 更新：200 OK 或 204 No Content
- 刪除：204 No Content
- 錯誤：ProblemDetails 包含適當的狀態碼

## 範例：ImageModelType 設計文件

請參閱 `docs/ImageModelType.md` 以取得結構良好的設計文件完整範例。

此範例的主要特點：
- 規劃和實作之間的清晰分離
- 所有階段都記錄了細項任務
- 問題/決策區段在實作前已解決
- 完整的 API 規格包含範例
- 實作狀態在文件中追蹤

## 交易處理指南

**重要：此專案不使用 Unit of Work 模式。所有交易管理都在儲存庫層處理。**

### 何時使用交易

當操作需要**原子性**時使用資料庫交易——多個資料庫操作必須全部成功或全部失敗。常見情境：
- 刪除現有記錄 + 新增新記錄（例如：重設操作）
- 更新多個必須保持一致的相關實體
- 涉及級聯變更的複雜操作

### 實作模式

**儲存庫層（處理交易）：**

```csharp
// 在儲存庫中
public async Task<(int deletedCount, List<Entity> createdEntities)> ComplexOperationAsync(
    string id,
    List<Entity> newEntities)
{
    // 在儲存庫中開始交易
    await using var transaction = await dbContext.Database.BeginTransactionAsync();

    try
    {
        // 1. 查詢並刪除
        var existing = await dbContext.Entities.Where(e => e.Id == id).ToListAsync();
        var deletedCount = existing.Count;
        dbContext.Entities.RemoveRange(existing);

        // 2. 新增
        await dbContext.Entities.AddRangeAsync(newEntities);

        // 3. 儲存所有變更
        await dbContext.SaveChangesAsync();

        // 4. 提交交易
        await transaction.CommitAsync();

        return (deletedCount, newEntities);
    }
    catch (Exception ex)
    {
        // 5. 錯誤時回滾
        await transaction.RollbackAsync();
        throw new InvalidOperationException($"操作失敗：{ex.Message}", ex);
    }
}
```

**服務層（不處理交易）：**

```csharp
// 在服務中
public async Task<OperationResult<ResultDto>> PerformComplexOperation(string id)
{
    // 1. 驗證和業務邏輯
    var entity = await _repository.GetByIdAsync(id);
    if (entity == null)
        return OperationResult<ResultDto>.Failure(404, "找不到");

    // 2. 準備資料
    var newEntities = PrepareNewEntities(entity);

    try
    {
        // 3. 呼叫儲存庫（交易在內部處理）
        var (deletedCount, created) = await _repository.ComplexOperationAsync(id, newEntities);

        // 4. 轉換為 DTO 並回傳
        var dto = MapToDto(created);
        return OperationResult<ResultDto>.Success(dto);
    }
    catch (InvalidOperationException ex)
    {
        // 5. 捕捉儲存庫例外並轉換為 OperationResult
        _logger.LogError(ex, "對 {Id} 執行操作失敗", id);
        return OperationResult<ResultDto>.Failure(500, ex.Message);
    }
}
```

### 規則

✅ **應該做：**
- 在儲存庫層使用 `DbContext.Database.BeginTransactionAsync()` 處理交易
- 將整個原子操作封裝在單一儲存庫方法中
- 使用 `try-catch` 搭配 `CommitAsync()` 和 `RollbackAsync()`
- 失敗時從儲存庫拋出例外
- 在服務中捕捉例外並轉換為 `OperationResult<T>`

❌ **不應該做：**
- 在服務層呼叫 `BeginTransactionAsync()`
- 從服務層直接存取 `DbContext`
- 將原子操作分散到多個儲存庫方法而不使用交易
- 在儲存庫層吞掉例外
- 從儲存庫層回傳 `OperationResult`（應改用例外）

## 最佳實踐

1. **從設計開始，而非程式碼**：務必先建立設計文件
2. **及早取得使用者意見**：在實作前呈現設計
3. **記錄決策**：在問題/決策區段記錄所有決策
4. **保持任務細項化**：將階段分解為具體、可執行的任務
5. **追蹤進度**：完成任務時更新勾選框
6. **記錄變更**：註記任何與原始設計的偏差
7. **包含範例**：提供完整的請求/回應範例
8. **考慮邊界情況**：清楚記錄驗證規則和業務規則
9. **交易處理**：始終在儲存庫層處理交易，絕不在服務層

## 良好設計文件的提示

- **具體明確**：避免模糊的術語；指定確切的類型、約束、長度
- **預先思考**：考慮與現有實體的關係
- **規劃成長**：設計可擴展的結構和 API
- **遵循模式**：使用現有程式碼庫模式以保持一致性
- **記錄假設**：將隱含的需求明確化
- **考慮效能**：思考索引、查詢模式
- **安全優先**：清楚記錄授權需求

## 實作期間執行的指令

```bash
# 建立遷移
dotnet ef migrations add Add{FeatureName} --project Dotnet10AISamples.Api

# 套用遷移
dotnet ef database update --project Dotnet10AISamples.Api

# 建置專案
dotnet build

# 執行測試
dotnet test

# 在本機執行 API
dotnet run --project Dotnet10AISamples.Api
```

## 最後提醒

- **絕不跳過設計階段** - 它能及早發現問題，節省時間
- **務必在撰寫程式碼前取得使用者同意**
- **隨著實作進展，保持設計文件更新**
- **將設計文件作為測試和文件的參考**
- **將設計文件存檔**在 `docs/` 資料夾中供未來參考