namespace Dotnet10AISamples.Api.Controllers.Users.Dtos;

public class UpdateUserDto
{
    /// <summary>
    /// 指出使用者帳戶是否啟用
    /// </summary>
    public bool? IsActive { get; set; }
}