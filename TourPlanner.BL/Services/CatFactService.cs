using System.Net.Http.Json;
using System.Text.Json;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.BL.Services;

public class CatFactService : ICatFactService
{
    private readonly HttpClient _httpClient;

    public CatFactService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CatFactApi");
    }

    public async Task<FactDto> CollectFactAsync(string username)
    {
        try
        {
            var factDto = await _httpClient.GetFromJsonAsync<FactDto>("fact");

            if (factDto == null)
            {
                throw new Exception(
                    "Received null fact DTO from Cat Facts API"
                );
            }

            return factDto;
        }
        // HttpRequestException -> non success status code
        // NotSupportedException -> content type is not valid
        // JsonException -> invalid JSON
        catch (Exception e)
            when (e is HttpRequestException
                  || e is NotSupportedException
                  || e is JsonException)
        {
            throw new Exception(
                "Failed to fetch fact from Cat Facts API",
                e
            );
        }
    }
}