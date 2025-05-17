using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace AgeGuesser.Api;

[Route("api/guess-age")]
[ApiController]
public class GuessAgeController(
    HttpClient httpClient,
    IMemoryCache memoryCache,
    IDistributedCache distributedCache,
    HybridCache hybridCache) : ControllerBase
{
    private record AgeGuessResponse(int Age);
    
    [HttpGet]
    public async Task<ActionResult<string>> GuessAge([FromQuery] string name)
    {
        var ageGuessResponse = await GetWithHybridCache(name);
        return Ok($"Hello {name}, your age is guessed to be around {ageGuessResponse.Age}.");
    }

    private async Task<AgeGuessResponse> GetWithMemoryCache(string name)
    {
        var cacheKey = $"guessedAge:{name}";
        var ageGuessResponse = await memoryCache.GetOrCreateAsync<AgeGuessResponse>(cacheKey, async _ =>
            await GetGuessedAge(name));

        return ageGuessResponse!;
    }
    
    private async Task<AgeGuessResponse> GetWithDistributedCache(string name)
    {
        var cacheKey = $"guessedAge:{name}";

        var ageGuessResponseJson = await distributedCache.GetStringAsync(cacheKey);
        if (ageGuessResponseJson is null)
        {
            var ageGuesserResponse = await GetGuessedAge(name);
            await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(ageGuesserResponse));
            return ageGuesserResponse;
        }

        return JsonSerializer.Deserialize<AgeGuessResponse>(ageGuessResponseJson)!;
    }

    private async Task<AgeGuessResponse> GetWithHybridCache(string name)
    {
        var cacheKey = $"guessedAge:{name}";
        var ageGuessResponse = await hybridCache.GetOrCreateAsync<AgeGuessResponse>(cacheKey, async _ =>
            await GetGuessedAge(name), new HybridCacheEntryOptions
        {
            Flags = HybridCacheEntryFlags.DisableDistributedCache
        });
        
        return ageGuessResponse;
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