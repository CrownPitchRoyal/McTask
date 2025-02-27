using UserManagement.Application.DTO.ApiKey;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Mappers;

public static class ApiKeyMapper
{
    public static ApiKeyDto ToDto(this ApiKey apiKey)
    {
        if (apiKey == null)
            throw new ArgumentNullException(nameof(apiKey), "ApiKey cannot be null");
        return new ApiKeyDto(
            apiKey.Key
        );
    }
}