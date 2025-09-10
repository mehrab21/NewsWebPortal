using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

public interface IGeoLocationService
{
    Task<string?> GetCountryByIPAsync(string ip);
}
public class GeoLocationService : IGeoLocationService
{
    private readonly HttpClient _httpClient;

    public GeoLocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetCountryByIPAsync(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return null;

        string url = $"http://ip-api.com/json/{ip}?fields=country";

        try
        {
            var response = await _httpClient.GetStringAsync(url);

            Console.WriteLine(response); // <-- see what API actually returns

            var json = JsonSerializer.Deserialize<IpApiResponse>(response);
            return json?.Country;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

}

public class IpApiResponse
{
    [JsonPropertyName("country")]
    public string Country { get; set; } = null!;
}
