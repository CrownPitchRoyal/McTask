namespace UserManagement.Application.DTO.ApiKey;

public class ApiKeyDto
{
    public Guid Key { get; set; }

    public ApiKeyDto()
    {
    }

    public ApiKeyDto(Guid key)
    {
        Key = key;
    }
}