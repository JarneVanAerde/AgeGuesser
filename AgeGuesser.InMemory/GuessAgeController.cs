using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AgeGuesser.InMemory;

[Route("api/guess-age")]
[ApiController]
public class GuessAgeController(HttpClient httpClient, IMemoryCache memoryCache) : ControllerBase
{
    private record AgeGuessResponse(int Age);
    
    [HttpGet]
    public async Task<ActionResult<string>> GuessAge([FromQuery] string name)
    {
        var cacheKey = $"guessedAge:{name}";
        var ageGuessResponse = await memoryCache.GetOrCreateAsync<AgeGuessResponse>(cacheKey, async _ =>
            await GetGuessedAge(name));
        
        return Ok($"Hello {name}, your age is guessed to be around {ageGuessResponse?.Age}.");
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