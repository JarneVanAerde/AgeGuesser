using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace AgeGuesser.Distributed;

[Route("api/guess-age")]
[ApiController]
public class GuessAgeController(HttpClient httpClient, IDistributedCache distributedCache) : ControllerBase
{
    private record AgeGuessResponse(int Age);
    
    [HttpGet]
    public async Task<ActionResult<string>> GuessAge([FromQuery] string name)
    {
        var cacheKey = $"guessedAge:{name}";

        var cacheResponse = await distributedCache.GetStringAsync(cacheKey);

        AgeGuessResponse response;
        if (cacheResponse is null)
        {
            response = await GetGuessedAge(name);
            await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response));
        }
        else
        {
            response = JsonSerializer.Deserialize<AgeGuessResponse>(cacheResponse)!;
        }
        
        return Ok($"Hello {name}, your age is guessed to be around {response.Age}.");
    }
    
    private async Task<AgeGuessResponse> GetGuessedAge(string name)
    {
        var result = await httpClient.GetAsync($"https://api.agify.io?name={name}");
        var content =  await result.Content.ReadFromJsonAsync<AgeGuessResponse>();
        if (content is null)
        {
            throw new InvalidOperationException("No guessed age returned");
        }

        return content;
    }
}