using System.Net.Http.Json;
using UserManagement.Application.DTO.ApiKey;
using UserManagement.Domain.Entities;

namespace UserManagement.API.IntegrationTests.Helpers;

public static class TestHttpClientExtensions
{
    /// <summary>
    /// Authenticates the HttpClient using the ApiKey login endpoint.
    /// This method sends a POST to /api/ApiKey with the given credentials,
    /// retrieves the API key from the response, and sets it in the client's default headers.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="userName">Username for authentication.</param>
    /// <param name="password">Password for authentication.</param>
    /// <returns>The API key as a string.</returns>
    public static async Task<string> AuthenticateAsnyc(this HttpClient client, string userName, string password)
    {
        // Prep login obj
        var loginDto = new LoginDto
        {
            userName = userName,
            password = password
        };
        
        // Post login details
        var response = await client.PostAsJsonAsync($"/api/ApiKey", loginDto);
        response.EnsureSuccessStatusCode();

        // Extract the api key
        var apiKey = await response.Content.ReadFromJsonAsync<ApiKeyDto>();
        Assert.NotNull(apiKey);
        Assert.NotNull(apiKey.Key.ToString());
        
        // Set the api key as header for subsequent requests
        client.DefaultRequestHeaders.Add("apikey", apiKey.Key.ToString());
        return apiKey.Key.ToString();
    }
}