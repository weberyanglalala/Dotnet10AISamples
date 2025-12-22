namespace Dotnet10AISamples.Api.Common;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public int Code { get; set; }
}