using System.Text;
using System.Text.Json;
using TourPlanner.BL.DTOs;
using TourPlanner.BL.Interfaces;
using Microsoft.Extensions.Configuration;
namespace TourPlanner.BL.Services;

public class OpenRouteService : IOpenRouteService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const string AutoCompleteUrl = "https://api.openrouteservice.org/geocode/autocomplete";
    private const string DirectionsUrl = "https://api.openrouteservice.org/v2/directions";
    

    public OpenRouteService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<Coords>> GetStringToCoordinate(string searchString)
    {
        var url =
            AutoCompleteUrl +
            $"?api_key={_configuration["OPENROUTE_APIKEY"]}" +
            $"&text={Uri.EscapeDataString(searchString)}" +
            $"&size=3";
        Console.WriteLine(url);
        var json = await _httpClient.GetStringAsync(url);
        
        using var doc = JsonDocument.Parse(json);

        var results = new List<Coords>();

        foreach (var feature in doc.RootElement.GetProperty("features").EnumerateArray())
        {
            var coords = feature.GetProperty("geometry").GetProperty("coordinates");
            var props = feature.GetProperty("properties");

            results.Add(new Coords
            {
                Name = props.GetProperty("label").GetString() ?? "",
                Lng = (decimal)coords[0].GetDouble(),
                Lat = (decimal)coords[1].GetDouble()
            });
        }

        return results;
    }

    public async Task<OpenRoute> GetRoute(OpenRoute openRoute)
    {
        var url = $"{DirectionsUrl}/{openRoute.TransportType}/geojson";

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        //for whatever reason, it does not like the the foromat of the key and throws a format exception
        request.Headers.TryAddWithoutValidation("Authorization", _configuration["OPENROUTE_APIKEY"]);
        Console.WriteLine(request.Headers.ToString());
        request.Headers.Add("Accept", "application/geo+json");
        
        var payload = new
        {
            coordinates = new[]
            {
                new[] { openRoute.ToFromCoords.FromCoord.Lng, openRoute.ToFromCoords.FromCoord.Lat },
                new[] { openRoute.ToFromCoords.ToCoord.Lng, openRoute.ToFromCoords.ToCoord.Lat }
            }
        };
        //i need to serialize it becuase of the stupid issue with comma vs dot
        //this cost me hours, i hate localization
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );
        
        var response = await _httpClient.SendAsync(request);
        try
        {
            Console.WriteLine("TRANSPORT TYPE "+openRoute.TransportType);
            Console.WriteLine("CORODINATE"+ openRoute.ToFromCoords.FromCoord.Lat);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        var feature = doc.RootElement
            .GetProperty("features")[0];

        var summary = feature
            .GetProperty("properties")
            .GetProperty("summary");

        var geometry = feature
            .GetProperty("geometry")
            .GetProperty("coordinates");

        openRoute.Distance = (decimal)summary.GetProperty("distance").GetDouble();
        openRoute.Duration = (decimal)summary.GetProperty("duration").GetDouble();
        


        foreach (var point in geometry.EnumerateArray())
        {
            openRoute.Steps.Add(new Coords
            {
                Lat = (decimal)point[0].GetDouble(),
                Lng = (decimal)point[1].GetDouble()
            });
        }

        return openRoute;
    }
    
}