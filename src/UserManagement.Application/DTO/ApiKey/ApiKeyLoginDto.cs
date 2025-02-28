namespace UserManagement.Application.DTO.ApiKey;

public class ApiKeyLoginDto
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }  // Optional: to communicate an error code if needed
    public string? Message { get; set; }
    public ApiKeyDto? ApiKey { get; set; }
}
