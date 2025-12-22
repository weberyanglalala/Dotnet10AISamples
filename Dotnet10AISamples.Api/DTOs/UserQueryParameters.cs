namespace Dotnet10AISamples.Api.DTOs;

public class UserQueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    /// <summary>
    /// 頁碼（從 1 開始）
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁項目數
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// 依啟用狀態篩選
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 是否套用啟用狀態篩選
    /// </summary>
    public bool IsActiveFilterEnabled { get; set; }

    /// <summary>
    /// 搜尋使用者名稱或電子郵件
    /// </summary>
    public string Search { get; set; }
}